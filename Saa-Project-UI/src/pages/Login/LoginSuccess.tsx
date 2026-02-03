import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import getCurrentUserId from "../../Services/GetCurrentUserId.ts";

const LoginSuccess = () => {

    const navigate = useNavigate();

    useEffect( () => {
        const handleLogin = async () => {
        const params = new URLSearchParams(window.location.search);
        const token = params.get('token');
        if (token) {
            try {
            localStorage.setItem('token', token);
            const userId = await getCurrentUserId();
            console.log(userId);
                if (userId) {
                    console.log("Google Login & Sync Successful!");
                    window.location.href = '/Posts';
                } else {
                    console.error("Failed to sync User ID from API");
                    navigate('/login');
                }
            } catch (error) {
                console.error("Invalid token:", error);
                navigate('/login');
            }
        } else {
            navigate('/login');
        }}
        handleLogin();
    }, [navigate]);

    return (
        <div style={{ textAlign: 'center', marginTop: '50px' }}>
            <h2>Connection in progress...</h2>
            <p>Please wait while we authenticate you.</p>
        </div>
    );
};

export default LoginSuccess;