import React from 'react';
import styles from './Inputs.module.css';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
    label: string;
    error?: string;
}
const Inputs: React.FC<InputProps> = ({ label, error, ...rest }) => {
    return (
        <div className={styles.container}>
            <label className={styles.label}>{label}</label>
            <input
                className={`${styles.inputField} ${error ? styles.errorInput : ''}`}
                {...rest}
            />
            {error && <span className={styles.errorMessage}>{error}</span>}
        </div>
    );
};
export default Inputs;