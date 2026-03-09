import React, {createContext, useContext, useState} from "react";

interface User {
    username: string;
}

interface AuthContextType {
    user: User | null;
    login: (username: string, password: string) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {
    const [user, setUser] = useState<User | null>(null);

    const login = async (username: string, password: string) => {

        const apiUrl = import.meta.env.VITE_API_URL;

        const res = await fetch(`${apiUrl}/api/auth/login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ username, password })
        });

        if (!res.ok) {
            throw new Error("Invalid credentials!");
        }

        const data = await res.json();
        localStorage.setItem("token", data.token);
        setUser({ username });
    };

    const logout = () => {
        localStorage.removeItem("token");
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{user, login, logout}}>
            {children}
        </AuthContext.Provider>
    );
    
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    
    if(!context){
        throw new Error("useAuth must be used within AuthProvider");
    }
    return context;
};