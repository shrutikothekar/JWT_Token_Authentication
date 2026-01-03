import React, { useState, useEffect } from 'react';
import { Routes, Route, Link, useNavigate, Navigate } from 'react-router-dom';
import { FaTachometerAlt, FaUserMd, FaSignOutAlt, FaBars, FaPlus, FaClinicMedical, FaUsers } from 'react-icons/fa';
import './Dashboard.css';

const Dashboard = () => {
    const [user, setUser] = useState(null);
    const [isSidebarCollapsed, setSidebarCollapsed] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const decodedToken = JSON.parse(atob(token.split('.')[1]));
                setUser({
                    name: decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
                    role: decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
                });
            } catch (error) {
                console.error('Error decoding token:', error);
                navigate('/login');
            }
        } else {
            navigate('/login');
        }
    }, [navigate]);

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    const toggleSidebar = () => {
        setSidebarCollapsed(!isSidebarCollapsed);
    };

    return (
        <div className="dashboard-container">
            <div className={`sidebar ${isSidebarCollapsed ? 'sidebar-collapsed' : ''}`}>
                <div className="sidebar-header">
                    {!isSidebarCollapsed && <h2>MediMeet</h2>}
                    <button onClick={toggleSidebar} className="sidebar-toggle-btn">
                        <FaBars />
                    </button>
                </div>
                <ul>
                    <li><Link to="/dashboard"><FaTachometerAlt /> {!isSidebarCollapsed && 'Dashboard'}</Link></li>
                    {/* <li><Link to="/master-data"><FaUserMd /> {!isSidebarCollapsed && 'User Data'}</Link></li> */}
                    
                </ul>
                <div className="logout-section">
                    <button onClick={handleLogout}><FaSignOutAlt /> {!isSidebarCollapsed && 'Logout'}</button>
                </div>
            </div>
            <div className="main-content">
                <Routes>
                    <Route path="dashboard" element={<DashboardContent user={user} />} />
                    {/* <Route path="master-data" element={<MasterData />} />
                    <Route path="manage-clinics" element={<ManageClinics />} />
                    <Route path="manage-specialties" element={<ManageSpecialties />} />
                    <Route path="manage-users" element={<ManageUsers />} />
                    <Route path="/*" element={<Navigate to="/dashboard" />} /> */}
                </Routes>
            </div>
        </div>
    );
};

const DashboardContent = ({ user }) => (
    <>
        <header>
            {user && (
                <div className="welcome-header">
                    <h1>Welcome, {user.name}</h1>
                    <p className="user-role">{user.role}</p>
                </div>
            )}
        </header>
        <div className="dashboard-widgets">
            <div className="widget">
                <h3>Upcoming Appointments</h3>
                <p>You have 5 upcoming appointments today.</p>
            </div>
            <div className="widget">
                <h3>Patient Queries</h3>
                <p>3 new messages from patients.</p>
            </div>
            <div className="widget">
                <h3>Clinic Schedule</h3>
                <p>Your next clinic is at 10:00 AM.</p>
            </div>
        </div>
    </>
);

export default Dashboard;