export interface BaseResponse<T> {
    isSuccess: boolean;
    data: T;
    message: string | null;
    errors: string[] | null;
}