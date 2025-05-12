
import React, { createContext, useState, useContext, useEffect } from "react";
import { toast } from "sonner";
import { User, LoginRequest, RegisterRequest, UpdateAccountRequest } from "../types";
import { authAPI } from "../services/api";

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (data: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<string>;
  logout: () => void;
  updateProfile: (data: UpdateAccountRequest) => Promise<void>;
  isAuthenticated: boolean;
  isModerator: boolean;
  isAdmin: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  // Check if user is authenticated on component mount
  useEffect(() => {
    const checkAuthStatus = async () => {
      try {
        const userData = await authAPI.getProfile();
        setUser(userData);
      } catch (error) {
        console.log("User not authenticated");
      } finally {
        setLoading(false);
      }
    };

    checkAuthStatus();
  }, []);

  // Login function
  const login = async (data: LoginRequest) => {
    setLoading(true);
    try {
      const token = await authAPI.login(data);
      console.log("Login successful, token received:", token);
      const userData = await authAPI.getProfile();
      setUser(userData);
      toast.success("Вход выполнен успешно!");
    } catch (error) {
      toast.error("Ошибка при входе. Проверьте email и пароль.");
      throw error;
    } finally {
      setLoading(false);
    }
  };

  // Register function
  const register = async (data: RegisterRequest) => {
    setLoading(true);
    try {
      const userId = await authAPI.register(data);
      toast.success("Регистрация выполнена успешно!");
      return userId;
    } catch (error) {
      toast.error("Ошибка при регистрации.");
      throw error;
    } finally {
      setLoading(false);
    }
  };

  // Logout function
  const logout = () => {
    // Clear cookie by setting it to expire
    document.cookie = "tasty-cookies=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    setUser(null);
    toast.success("Вы вышли из системы");
  };

  // Update profile
  const updateProfile = async (data: UpdateAccountRequest) => {
    setLoading(true);
    try {
      await authAPI.updateProfile(data);
      const updatedUser = await authAPI.getProfile();
      setUser(updatedUser);
      toast.success("Профиль успешно обновлен");
    } catch (error) {
      toast.error("Ошибка при обновлении профиля");
      throw error;
    } finally {
      setLoading(false);
    }
  };

  // Computed properties
  const isAuthenticated = !!user;
  const isModerator = isAuthenticated && (user?.role === "Moderator" || user?.role === "Admin");
  const isAdmin = isAuthenticated && user?.role === "Admin";

  const value = {
    user,
    loading,
    login,
    register,
    logout,
    updateProfile,
    isAuthenticated,
    isModerator,
    isAdmin,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
