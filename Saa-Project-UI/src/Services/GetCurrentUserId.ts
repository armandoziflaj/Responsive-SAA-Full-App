import AuthService from "./AuthService.ts";

const GetCurrentUserId = async () => {
    try {
        const response = await AuthService.get('api/Account/GetCurrentUser');

        const userId = response.data.data.userId;

        if (userId) {
            localStorage.setItem('userId', userId.toString());
            console.log('User ID sync successful:', userId);
            return userId;
        }
    } catch (err) {
        console.error("Error fetching current user:", err);
    }
    return null;
};
export default GetCurrentUserId;