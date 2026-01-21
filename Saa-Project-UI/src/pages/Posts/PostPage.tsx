import React, { useEffect, useState } from 'react';
import api from '../../Services/AuthService';
import type {BaseResponse} from '../../Types/ApiResponses';
import type { PostResponse } from '../../Types/PostResponse';
import styles from './PostPage.module.css';

const Posts: React.FC = () => {
    const [posts, setPosts] = useState<PostResponse[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const loadPosts = async () => {
            try {
                const response = await api.get<BaseResponse<PostResponse[]>>('/api/Posts');
                if (response.data.isSuccess) {
                    setPosts(response.data.data);
                }
            } catch (error) {
                console.error("Error while loading posts", error);
            } finally {
                setLoading(false);
            }
        };
        loadPosts();
    }, []);

    if (loading) return <div className={styles.spinner}>Loading...</div>;

    return (
        <div className={styles.container}>
            <header className={styles.header}>
                <h1>Community Feed</h1>
                <p>See the latest posts from our community</p>
            </header>

            <div className={styles.postsGrid}>
                {posts.map(post => (
                    <article key={post.id} className={styles.card}>
                        <div className={styles.cardHeader}>
                            <span className={styles.categoryTag}>{post.category}</span>
                            <span className={styles.date}> {new Date(post.createdOn).toLocaleDateString()}</span>
                        </div>
                        <h2 className={styles.title}>{post.title}</h2>
                        <p className={styles.content}>
                            {post.content.length > 150
                                ? post.content.substring(0, 150) + "..."
                                : post.content}
                        </p>
                        <div className={styles.cardFooter}>
                            <span className={styles.author}>Author: <strong>{post.authorName}</strong></span>
                            <button className={styles.readMore}>Read More</button>
                        </div>
                    </article>
                ))}
            </div>
        </div>
    );
};

export default Posts;