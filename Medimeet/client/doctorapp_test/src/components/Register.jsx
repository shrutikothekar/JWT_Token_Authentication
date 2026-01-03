import React, { useState, useEffect } from 'react';
import axios from '../api/axios';
import './Register.css';
import { Link, useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';

const Register = () => {
    const [formData, setFormData] = useState({
        fullName: '',
        email: '',
        password: '',
        role: '1', // Default role
    });
    const [roles, setRoles] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchRoles = async () => {
            try {
                const response = await axios.get('/Auth/roles');
                setRoles(response.data);
                console.log('Roles from API:', response.data);
            } catch (error) {
                console.error('Error fetching roles:', error);
                toast.error('Failed to load roles. Please try again later.');
            }
        };

        fetchRoles();
    }, []);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const dataToSend = {
            ...formData,
            role: parseInt(formData.role, 10),
        };

        console.log('Data being sent to API:', dataToSend);

        try {
            const response = await axios.post('/Auth/register', dataToSend);
            localStorage.setItem('token', response.data.token);
            toast.success('Registration successful! Redirecting to dashboard...');
            navigate('/dashboard');
        } catch (error) {
            if (error.response && error.response.data) {
                toast.error(error.response.data);
            } else {
                toast.error('An error occurred. Please try again.');
            }
        }
    };

    return (
        <div class="register-container">
            <form onSubmit={handleSubmit} className="register-form">
                <h2>Register</h2>
                <div className="form-group">
                    <label>Full Name</label>
                    <input
                        type="text"
                        name="fullName"
                        value={formData.fullName}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div className="form-group">
                    <label>Email</label>
                    <input
                        type="email"
                        name="email"
                        value={formData.email}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div className="form-group">
                    <label>Password</label>
                    <input
                        type="password"
                        name="password"
                        value={formData.password}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div className="form-group">
                    <label>Role</label>
                    <select name="role" value={formData.role} onChange={handleChange}>
                        {roles.map((role) => (
                            <option key={role} value={role}>
                                {role === 0 ? 'Patient' : role === 1 ? 'Doctor' : 'Admin'}
                            </option>
                        ))}
                    </select>
                </div>
                <button type="submit">Register</button>
                <p>
                    Already have an account? <Link to="/login">Login</Link>
                </p>
            </form>
        </div>
    );
};

export default Register;