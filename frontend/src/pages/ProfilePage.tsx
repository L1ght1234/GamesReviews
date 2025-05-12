
import React, { useState } from "react";
import Layout from "../components/layout/Layout";
import { useAuth } from "../context/AuthContext";
import { toast } from "sonner";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Lock } from "lucide-react";

const ProfilePage: React.FC = () => {
  const { user, updateProfile } = useAuth();
  
  const [userName, setUserName] = useState(user?.userName || "");
  const [email, setEmail] = useState(user?.email || "");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [changePassword, setChangePassword] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Always require current password for any change
    if (!currentPassword) {
      toast.error("Введите текущий пароль для изменения данных аккаунта");
      return;
    }
    
    if (changePassword && newPassword !== confirmNewPassword) {
      toast.error("Новые пароли не совпадают");
      return;
    }

    setIsLoading(true);

    try {
      const updateData = {
        userName: userName !== user?.userName ? userName : user?.userName,
        email: email !== user?.email ? email : user?.email,
        currentPassword,
        newPassword: changePassword ? newPassword : undefined,
      };
      
      // Check if there are any actual changes
      const hasChanges = userName !== user?.userName || 
                        email !== user?.email || 
                        (changePassword && newPassword);
      
      if (hasChanges) {
        await updateProfile(updateData);
        // Clear password fields after successful update
        setCurrentPassword("");
        if (changePassword) {
          setNewPassword("");
          setConfirmNewPassword("");
          setChangePassword(false);
        }
        toast.success("Профиль успешно обновлен");
      } else {
        toast.info("Нет изменений для сохранения");
      }
    } catch (error) {
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  };

  if (!user) {
    return (
      <Layout>
        <div className="text-center py-8">
          <p>Необходимо войти в систему для доступа к профилю.</p>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-2xl mx-auto">
        <div className="bg-card border border-border rounded-lg shadow-lg p-6">
          <h1 className="text-2xl font-bold mb-6">Профиль пользователя</h1>
          
          <div className="mb-6 p-4 bg-muted rounded-lg">
            <h2 className="font-medium mb-2">Информация об аккаунте:</h2>
            <p>Имя пользователя: {user.userName}</p>
            <p>Email: {user.email}</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Current password field - always required */}
            <div>
              <div className="flex items-center mb-2">
                <Lock size={16} className="mr-2 text-orange-500" />
                <Label htmlFor="currentPassword" className="text-orange-500">
                  Текущий пароль (обязательно для любых изменений)
                </Label>
              </div>
              <Input
                id="currentPassword"
                type="password"
                placeholder="Введите текущий пароль"
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
                className="border-orange-300 focus-visible:ring-orange-400"
                required
              />
            </div>
            
            <div className="space-y-4">
              {/* New username field */}
              <div>
                <Label htmlFor="userName">Новое имя пользователя</Label>
                <Input
                  id="userName"
                  type="text"
                  value={userName}
                  onChange={(e) => setUserName(e.target.value)}
                  placeholder={user.userName}
                />
              </div>

              {/* New email field */}
              <div>
                <Label htmlFor="email">Новый email</Label>
                <Input
                  id="email"
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder={user.email}
                />
              </div>
            </div>

            <div className="border-t border-border pt-4">
              {/* Toggle for password change */}
              <div className="flex items-center mb-4">
                <input
                  type="checkbox"
                  id="changePassword"
                  checked={changePassword}
                  onChange={(e) => setChangePassword(e.target.checked)}
                  className="mr-2 h-4 w-4"
                />
                <Label htmlFor="changePassword">
                  Изменить пароль
                </Label>
              </div>

              {/* Conditional password change fields */}
              {changePassword && (
                <div className="space-y-4">
                  <div>
                    <Label htmlFor="newPassword">
                      Новый пароль
                    </Label>
                    <Input
                      id="newPassword"
                      type="password"
                      value={newPassword}
                      onChange={(e) => setNewPassword(e.target.value)}
                      required={changePassword}
                    />
                  </div>

                  <div>
                    <Label htmlFor="confirmNewPassword">
                      Подтвердите новый пароль
                    </Label>
                    <Input
                      id="confirmNewPassword"
                      type="password"
                      value={confirmNewPassword}
                      onChange={(e) => setConfirmNewPassword(e.target.value)}
                      required={changePassword}
                    />
                  </div>
                </div>
              )}
            </div>

            <button
              type="submit"
              disabled={isLoading}
              className="btn-primary w-full flex items-center justify-center"
            >
              {isLoading ? (
                <svg 
                  className="animate-spin h-5 w-5" 
                  xmlns="http://www.w3.org/2000/svg" 
                  fill="none" 
                  viewBox="0 0 24 24"
                >
                  <circle 
                    className="opacity-25" 
                    cx="12" 
                    cy="12" 
                    r="10" 
                    stroke="currentColor" 
                    strokeWidth="4"
                  ></circle>
                  <path 
                    className="opacity-75" 
                    fill="currentColor" 
                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                  ></path>
                </svg>
              ) : "Сохранить изменения"}
            </button>
          </form>
        </div>
      </div>
    </Layout>
  );
};

export default ProfilePage;
