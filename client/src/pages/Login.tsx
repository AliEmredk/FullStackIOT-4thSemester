import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { Eye, EyeOff } from "lucide-react";

const Login = () => {
    const { login } = useAuth();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [error, setError] = useState("");
    
    const handleLogin = async () => {
        try {
            await login(username, password);
        } catch {
            setError("Invalid username or password");
        }
    };
    
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        await handleLogin();
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-slate-900">
            <form
                onSubmit={handleSubmit}
                className="bg-slate-800 border border-slate-800 rounded-2xl p-10 w-96 shadow-xl"
            >
                <h1 className="text-2xl font-bold mb-6 text-center text-blue-400">
                    Offshore Wind Farm Login
                </h1>

                <input
                    className="w-full mb-4 p-3 rounded-lg bg-slate-700 border border-slate-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                />

                <div className="relative mb-6">
                    <input
                        type={showPassword ? "text" : "password"}
                        className="w-full p-3 rounded-lg bg-slate-700 border border-slate-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />

                    <button
                        type="button"
                        onClick={() => setShowPassword(!showPassword)}
                        className="absolute right-3 top-1/2 -translate-y-1/2 flex items-center text-slate-400 hover:text-blue-400 transition"
                    >
                        {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                    </button>
                </div>

                <button
                    type="submit"
                    disabled={!username || !password}
                    className="w-full bg-blue-600 hover:bg-blue-500 transition rounded-lg py-3 font-semibold"
                >
                    Sign In
                </button>

                {error && (
                    <p className="text-red-400 text-sm mb-4 text-center">{error}</p>
                )}
            </form>
        </div>
    );
};

export default Login;