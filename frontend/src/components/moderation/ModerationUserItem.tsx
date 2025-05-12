
import React, { useState } from "react";
import { Edit, Trash, X, Check } from "lucide-react";
import { toast } from "sonner";
import { User } from "../../types";
import { moderationAPI } from "../../services/api";
import { useAuth } from "../../context/AuthContext";

interface ModerationUserItemProps {
  user: User;
  onUserUpdated: () => void;
  onUserDeleted: () => void;
}

const ModerationUserItem: React.FC<ModerationUserItemProps> = ({
  user,
  onUserUpdated,
  onUserDeleted,
}) => {
  const { user: currentUser } = useAuth();
  const [isEditing, setIsEditing] = useState(false);
  const [userName, setUserName] = useState(user.userName);
  const [email, setEmail] = useState(user.email);
  const [newPassword, setNewPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const isCurrentUser = currentUser?.id === user.id;
  const isAdmin = user.role === "Admin";

  const handleUpdate = async () => {
    if (!userName.trim() || !email.trim()) {
      toast.error("Имя пользователя и email не могут быть пустыми");
      return;
    }

    setIsLoading(true);
    try {
      await moderationAPI.updateUser(user.id, {
        userName,
        email,
        newPassword: newPassword || undefined,
      });
      setIsEditing(false);
      onUserUpdated();
    } catch (error) {
      toast.error("Не удалось обновить пользователя");
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async () => {
    if (isCurrentUser) {
      toast.error("Нельзя удалить текущего пользователя");
      return;
    }

    if (isAdmin) {
      toast.error("Нельзя удалить администратора");
      return;
    }

    if (window.confirm(`Вы уверены, что хотите удалить пользователя ${user.userName}?`)) {
      setIsLoading(true);
      try {
        await moderationAPI.deleteUser(user.id);
        onUserDeleted();
      } catch (error) {
        toast.error("Не удалось удалить пользователя");
      } finally {
        setIsLoading(false);
      }
    }
  };

  const handleCancel = () => {
    setUserName(user.userName);
    setEmail(user.email);
    setNewPassword("");
    setIsEditing(false);
  };

  // Determine if edit/delete buttons should be disabled
  const deleteDisabled = isCurrentUser || isAdmin || isLoading;
  const editDisabled = isAdmin || isLoading;

  return (
    <tr className="bg-card hover:bg-muted/30">
      <td className="px-4 py-3">
        {isEditing ? (
          <input
            type="text"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            className="bg-muted p-2 rounded w-full"
          />
        ) : (
          user.userName
        )}
      </td>
      <td className="px-4 py-3">
        {isEditing ? (
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="bg-muted p-2 rounded w-full"
          />
        ) : (
          user.email
        )}
      </td>
      <td className="px-4 py-3">
        <span
          className={`px-2 py-1 rounded text-xs ${
            user.role === "Admin"
              ? "bg-primary/20 text-primary"
              : user.role === "Moderator"
              ? "bg-accent/20 text-accent"
              : "bg-muted text-muted-foreground"
          }`}
        >
          {user.role}
        </span>
      </td>
      <td className="px-4 py-3 text-right">
        {isEditing ? (
          <>
            <div className="mb-2">
              <input
                type="password"
                placeholder="Новый пароль (необязательно)"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                className="bg-muted p-2 rounded w-full text-xs"
              />
            </div>
            <div className="flex justify-end space-x-2">
              <button
                onClick={handleCancel}
                className="p-1 hover:text-primary"
                disabled={isLoading}
              >
                <X size={16} />
              </button>
              <button
                onClick={handleUpdate}
                className="p-1 hover:text-accent"
                disabled={isLoading}
              >
                <Check size={16} />
              </button>
            </div>
          </>
        ) : (
          <div className="flex justify-end space-x-2">
            <button
              onClick={() => setIsEditing(true)}
              className={`p-1 ${
                editDisabled
                  ? "text-muted-foreground/50 cursor-not-allowed"
                  : "hover:text-accent"
              }`}
              disabled={editDisabled}
              title={
                isAdmin
                  ? "Нельзя изменить администратора"
                  : "Изменить пользователя"
              }
            >
              <Edit size={16} />
            </button>
            <button
              onClick={handleDelete}
              className={`p-1 ${
                deleteDisabled
                  ? "text-muted-foreground/50 cursor-not-allowed"
                  : "hover:text-destructive"
              }`}
              disabled={deleteDisabled}
              title={
                isCurrentUser
                  ? "Нельзя удалить текущего пользователя"
                  : isAdmin
                  ? "Нельзя удалить администратора"
                  : "Удалить пользователя"
              }
            >
              <Trash size={16} />
            </button>
          </div>
        )}
      </td>
    </tr>
  );
};

export default ModerationUserItem;
