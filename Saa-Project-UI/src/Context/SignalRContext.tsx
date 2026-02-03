import { createContext, useContext } from 'react';
import * as signalR from "@microsoft/signalr";

export interface SignalRContextType {
    connection: signalR.HubConnection | null;
    isJoined: boolean;
}

export const SignalRContext = createContext<SignalRContextType>({
    connection: null,
    isJoined: false
});

export const useSignalR = () => useContext(SignalRContext);