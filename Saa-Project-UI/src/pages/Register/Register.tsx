import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import Input from '../../components/Inputs/Inputs.tsx';
import Button from '../../components/Button/Button.tsx';
import styles from '..//Login/Login.module.css'; //
import AuthService from "../../Services/AuthService.ts";
import type { AxiosError } from "axios";

const Register: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();

        if (password !== confirmPassword) {
            setError('Passwords do not match!');
            return;
        }

        setLoading(true);
        setError('');

        try {
            await AuthService.post('register', { email, password });

            console.log('Registration successful');
            navigate('/login');
        } catch (err) {
            const axiosError = err as AxiosError<{ message: string }>;
            const errorMessage = axiosError.response?.data?.message || 'Registration failed. Please try again.';
            setError(errorMessage);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className={styles.loginPage} onSubmit={handleRegister}>
            <form className={styles.formCard}>
                <h2>Create Account</h2>
                <p>Join our platform today</p>

                <Input
                    label="Email"
                    type="email"
                    placeholder="e.g. user@example.com"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />

                <Input
                    label="Password"
                    type="password"
                    placeholder="Create a strong password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />

                <Input
                    label="Confirm Password"
                    type="password"
                    placeholder="Repeat your password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                />

                {error && <p className={styles.errorMessage}>{error}</p>}

                <Button type="submit" disabled={loading}>
                    {loading ? 'Creating account...' : 'Register'}
                </Button>

                <div className={styles.footer}>
                    Already have an account? <Link to="/login">Login here</Link>
                </div>
            </form>
        </div>
    );
};

export default Register;