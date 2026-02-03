import React, { useEffect, useState } from 'react';
import api from '../../Services/AuthService';
import PostCard from '../../components/PostCard/PostCard';
import type { BaseResponse } from '../../Types/ApiResponses';
import type { PostResponse } from '../../Types/PostResponse';
import styles from './PostPage.module.css';
import PostModal from "../../components/CreatePost/PostModal.tsx";

const Posts: React.FC = () => {
    const [posts, setPosts] = useState<PostResponse[]>([]);
    const [loading, setLoading] = useState(true);

    const [selectedPost, setSelectedPost] = useState<PostResponse | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const currentUserId = parseInt(localStorage.getItem('userId') || '0');

    useEffect(() => {
        loadPosts();
    }, []);

    const loadPosts = async () => {
        try {
            const response = await api.get<BaseResponse<PostResponse[]>>('/api/Posts');
            if (response.data.isSuccess) {
                setPosts(response.data.data);
            }
        } catch (error) {
            console.error("Error loading posts", error);
        } finally {
            setLoading(false);
        }
    };

    const handleSavePost = async (formData: { title: string; category: string; content: string }) => {
        try {
            if (selectedPost) {
                const response = await api.put<BaseResponse<string>>(`/api/Posts`, {
                    id: selectedPost.id,
                    ...formData
                });

                if (response.data.isSuccess) {
                    setPosts(prev => prev.map(p => p.id === selectedPost.id ? { ...p, ...formData } : p));
                    alert("Post updated!");
                }
            } else {
                const response = await api.post<BaseResponse<PostResponse>>(`/api/Posts`, formData);
                if (response.data.isSuccess) {
                    loadPosts();
                    alert("Post created!");
                }
            }
            setIsModalOpen(false);
        } catch (error) {
            console.error("Operation failed", error);
        }
    };

    const handleDeletePost = async (postId: number) => {
        try {
            const response = await api.delete<BaseResponse<string>>(`/api/Posts/${postId}`);
            if (response.data.isSuccess) {
                setPosts(prev => prev.filter(p => p.id !== postId));
                setIsModalOpen(false);
                alert("Post deleted!");
            }
        } catch (error) {
            console.error("Delete failed", error);
        }
    };

    const handleAddContact = async (contactId: number) => {
        try {
            const response = await api.post(`/api/Contacts/${contactId}`);
            if (response.data.isSuccess) alert("Contact added!");
        } catch (error) {
            console.error("Add contact failed", error);
        }
    };

    if (loading) return <div className={styles.spinner}>Loading Community Feed...</div>;

    return (
        <div className={styles.container}>
            <header className={styles.header}>
                <div className={styles.titleGroup}>
                    <h1>Community Feed</h1>
                    <p>Manage your posts or connect with others</p>
                </div>
                {currentUserId !== 0 && (
                <button
                    className={styles.openAddModalBtn}
                    onClick={() => { setSelectedPost(null); setIsModalOpen(true); }}
                >
                    <i className="fa fa-plus"></i> New Post
                </button>
                )}
            </header>

            <div className={styles.postsGrid}>
                {posts.map(post => (
                    <PostCard
                        key={post.id}
                        post={post}
                        currentUserId={currentUserId}
                        onAddContact={handleAddContact}
                        onEditClick={(p) => { setSelectedPost(p); setIsModalOpen(true); }}
                    />
                ))}
            </div>

            <PostModal
                key={selectedPost?.id || 'new'}
                isOpen={isModalOpen}
                initialData={selectedPost}
                onClose={() => setIsModalOpen(false)}
                onSave={handleSavePost}
                onDelete={handleDeletePost}
            />
        </div>
    );
};

export default Posts;