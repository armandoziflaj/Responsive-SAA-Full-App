import React from 'react';
import styles from './PostCard.module.css';

export interface Post {
    id: number;
    title: string;
    content: string;
    category: string;
    authorId: number;
    authorName: string;
    createdOn: string;
}

interface PostCardProps {
    post: Post;
    currentUserId: number | null;
    onAddContact: (id: number) => void;
    // Πλέον περνάμε ολόκληρο το post στην onEditClick
    onEditClick: (post: Post) => void;
}

const PostCard: React.FC<PostCardProps> = ({ post, currentUserId, onAddContact, onEditClick }) => {
    const isMine = currentUserId !== 0 &&  currentUserId === post.authorId;
    const isNotMine = currentUserId !== 0 && currentUserId !== post.authorId;

    return (
        <div className={styles.card}>
            <div className={styles.cardHeader}>
                <span className={styles.categoryTag}>{post.category}</span>
                <h3 className={styles.title}>{post.title}</h3>
            </div>

            <p className={styles.description}>{post.content}</p>

            <div className={styles.cardFooter}>
                <div className={styles.authorInfo}>
                    <span className={styles.authorName}>From: <strong>{post.authorName}</strong></span>
                    <span className={styles.date}> {new Date(post.createdOn).toLocaleDateString()}</span>
                </div>

                {isNotMine && currentUserId !== 0 && (
                    <button
                        className={styles.addContactBtn}
                        onClick={() => onAddContact(post.authorId)}
                        title="Add to contacts"
                    >
                        <i className="fa fa-plus" aria-hidden="true"></i>
                    </button>
                )}

                {isMine && currentUserId !== 0 && (
                    <div className={styles.myActions}>
                        <button
                            className={styles.editBtn}
                            onClick={() => onEditClick(post)}
                            title="Edit or Delete post"
                        >
                            <i className="fa fa-pencil" aria-hidden="true"></i>
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
};

export default PostCard;