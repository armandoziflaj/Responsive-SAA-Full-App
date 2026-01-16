import { useState, useEffect } from 'react';

interface Post {
  id: number;
  title: string;
  content: string;
  author: string;
}

const PostsPage = () => {
  const [posts, setPosts] = useState<Post[]>([]);

  useEffect(() => {
    // 2. Εδώ προσομοιώνουμε τη λήψη δεδομένων (αργότερα θα μπει το fetch στο backend)
    const dummyPosts: Post[] = [
      { id: 1, title: 'Καλωσήρθατε', content: 'Αυτό είναι το πρώτο post.', author: 'Admin' },
      { id: 2, title: 'Εργαστήριο', content: 'Ψάχνω άτομα για την εργασία.', author: 'Γιάννης' },
    ];

      // eslint-disable-next-line react-hooks/set-state-in-effect
    setPosts(dummyPosts);
  }, []);

  return (
    <div style={{ padding: '1rem' }}>
      <h2>Αναρτήσεις (Posts)</h2>
      
      {/* 3. Εμφανίζουμε τη λίστα με τα posts */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
        {posts.map((post) => (
          <div key={post.id} style={{ border: '1px solid #ddd', padding: '1rem', borderRadius: '8px' }}>
            <h3>{post.title}</h3>
            <p>{post.content}</p>
            <small>Από: {post.author}</small>
          </div>
        ))}
      </div>
    </div>
  );
};

export default PostsPage;
