import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Input from '../../components/Inputs/Inputs.tsx';
import Button from '../../components/Button/Button.tsx';
import styles from './Login.module.css';
import AuthService from "../../Services/AuthService.ts";
import type {AxiosError} from "axios";

const Login: React.FC = () => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

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
            }
            navigate('/');
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
        <div className={styles.loginPage} onSubmit={handleLogin}>
            <form className={styles.formCard}>
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
            </form>
        </div>
    );
};

export default Login;