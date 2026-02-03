import React, { useState } from 'react';
import Input from '../../components/Inputs/Inputs.tsx';
import Button from '../../components/Button/Button.tsx';
import styles from './Login.module.css';
import AuthService from "../../Services/AuthService.ts";
import GetCurrentUserId from "../../Services/GetCurrentUserId.ts"
import type {AxiosError} from "axios";
import {Link} from "react-router-dom";


const Login: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleGoogleLogin = async () => {
        window.location.href = "http://localhost:5245/api/Google/login";
    };
    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const response = await AuthService.post('login', { email, password });

            const token = response.data.accessToken;
            if (token) {
                localStorage.setItem('token', token);
                console.log('Login Successful, token stored!');
                await GetCurrentUserId();
            }
            window.location.href = '/Posts';
        } catch (err) {const axiosError = err as AxiosError<{ message: string }>;

            const errorMessage = axiosError.response?.data?.message
                || 'Email or password are wrong.';

            setError(errorMessage);
            console.error("Login Error:", axiosError);
        } finally {
            setLoading(false);
        }
    };
    return (
        <div className={styles.loginPage} >
            <form className={styles.formCard} onSubmit={handleLogin}>
                <h2>Login</h2>

                <Input
                    label="Email"
                    type="email"
                    placeholder="e.g. user@example.com"
                    value={email}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setEmail(e.target.value)}
                />

                <Input
                    label="Password"
                    type="password"
                    placeholder="Type your password"
                    value={password}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setPassword(e.target.value)} // Χρησιμοποιούμε τη setPassword
                    required
                />
                {error && <p className={styles.errorMessage}>{error}</p>}
                <Button type="submit">
                    {loading ? 'Loading...' : 'Login'}
                </Button>

                <hr className={styles.divider} />

                <button
                type="button"
                onClick={handleGoogleLogin}
                className={styles.googleButton}
                >
                <i className="fa-brands fa-google" style={{ color: '#DB4437' }}></i>
                Continue with Google
                </button>

                <div className={styles.footer}>
                    Don't have an account yet? <Link to="/Register">Register here</Link>
                </div>
            </form>
        </div>
    );
};

export default Login;