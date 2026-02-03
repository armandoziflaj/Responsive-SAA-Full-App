import React, { useState } from 'react';
import styles from './PostModal.module.css';
import type { PostResponse } from '../../Types/PostResponse';

interface PostModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSave: (data: { title: string; category: string; content: string }) => void;
    onDelete?: (id: number) => void;
    initialData?: PostResponse | null;
}

const PostModal: React.FC<PostModalProps> = ({ isOpen, onClose, onSave, onDelete, initialData }) => {
    const [formData, setFormData] = useState({
        title: initialData?.title || '',
        category: initialData?.category || '',
        content: initialData?.content || ''
    });

    if (!isOpen) return null;

    return (
        <div className={styles.modalOverlay}>
            <div className={styles.modalContent}>
                <h3>{initialData ? 'Edit Post' : 'Create New Post'}</h3>

                <div className={styles.formGroup}>
                    <label>Title</label>
                    <input
                        className={styles.modalInput}
                        value={formData.title}
                        onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                        placeholder="Enter title..."
                    />
                </div>

                <div className={styles.formGroup}>
                    <label>Category</label>
                    <input
                        className={styles.modalInput}
                        value={formData.category}
                        onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                        placeholder="e.g. Technology"
                    />
                </div>

                <div className={styles.formGroup}>
                    <label>Content</label>
                    <textarea
                        className={styles.modalTextarea}
                        rows={5}
                        value={formData.content}
                        onChange={(e) => setFormData({ ...formData, content: e.target.value })}
                        placeholder="What's on your mind?"
                    />
                </div>

                <div className={styles.modalActions}>
                    <div className={styles.saveCancelGroup}>
                        <button onClick={() => onSave(formData)} className={styles.saveBtn}>
                            {initialData ? 'Save Changes' : 'Create Post'}
                        </button>
                        <button onClick={onClose} className={styles.cancelBtn}>Cancel</button>
                    </div>

                    {initialData && onDelete && (
                        <button
                            onClick={() => window.confirm("Delete this post?") && onDelete(initialData.id)}
                            className={styles.deleteBtnModal}
                        >
                            <i className="fa fa-trash"></i> Delete
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
};

export default PostModal;