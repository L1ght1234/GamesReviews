
import React, { useState, useEffect } from "react";
import { toast } from "sonner";
import { Search } from "lucide-react";
import Pagination from "../shared/Pagination";
import { moderationAPI } from "../../services/api";
import { User, UserFilterRequest } from "../../types";
import ModerationUserItem from "./ModerationUserItem";

const ModerationUsersList: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState("");
  const [roleFilter, setRoleFilter] = useState<string>("");
  const [sortBy, setSortBy] = useState("UserName");
  const [sortDirection, setSortDirection] = useState("asc");

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const filter: UserFilterRequest = {
        search: searchTerm,
        role: roleFilter || undefined,
        sortBy,
        sortDirection,
        page: currentPage,
        pageSize,
      };

      const response = await moderationAPI.getUsers(filter);
      
      setUsers(response.items);
      setTotalCount(response.totalCount);
      setError("");
    } catch (err) {
      console.error("Error fetching users:", err);
      setError("Не удалось загрузить список пользователей");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [currentPage, sortBy, sortDirection, roleFilter]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setCurrentPage(1);
    fetchUsers();
  };

  const handleRoleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setRoleFilter(e.target.value);
    setCurrentPage(1);
  };

  const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSortBy(e.target.value);
    setCurrentPage(1);
  };

  const handleDirectionChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSortDirection(e.target.value);
    setCurrentPage(1);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const handleUserUpdated = () => {
    toast.success("Пользователь успешно обновлен");
    fetchUsers();
  };

  const handleUserDeleted = () => {
    toast.success("Пользователь успешно удален");
    fetchUsers();
  };

  return (
    <div>
      <div className="mb-6">
        <form onSubmit={handleSearch} className="flex flex-wrap gap-4">
          <div className="relative flex-grow">
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Поиск пользователей..."
              className="w-full p-3 pr-10 bg-muted rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
            />
            <button
              type="submit"
              className="absolute right-3 top-1/2 transform -translate-y-1/2 text-muted-foreground hover:text-primary"
            >
              <Search size={20} />
            </button>
          </div>
          
          <div className="flex gap-2">
            <select
              value={roleFilter}
              onChange={handleRoleChange}
              className="bg-muted p-3 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="">Все роли</option>
              <option value="User">User</option>
              <option value="Moderator">Moderator</option>
              <option value="Admin">Admin</option>
            </select>
            
            <select
              value={sortBy}
              onChange={handleSortChange}
              className="bg-muted p-3 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="UserName">По имени</option>
              <option value="Email">По email</option>
              <option value="Role">По роли</option>
            </select>
            
            <select
              value={sortDirection}
              onChange={handleDirectionChange}
              className="bg-muted p-3 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="asc">По возрастанию</option>
              <option value="desc">По убыванию</option>
            </select>
          </div>
        </form>
      </div>

      {loading ? (
        <div className="flex justify-center py-12">
          <div className="animate-spin h-12 w-12 border-4 border-primary border-t-transparent rounded-full"></div>
        </div>
      ) : error ? (
        <div className="bg-destructive/10 text-destructive p-4 rounded-md">
          {error}
        </div>
      ) : users.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-muted-foreground text-lg">Пользователей не найдено</p>
        </div>
      ) : (
        <>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-muted">
                <tr>
                  <th className="px-4 py-3 text-left">Имя пользователя</th>
                  <th className="px-4 py-3 text-left">Email</th>
                  <th className="px-4 py-3 text-left">Роль</th>
                  <th className="px-4 py-3 text-right">Действия</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {users.map((user) => (
                  <ModerationUserItem
                    key={user.id}
                    user={user}
                    onUserUpdated={handleUserUpdated}
                    onUserDeleted={handleUserDeleted}
                  />
                ))}
              </tbody>
            </table>
          </div>

          <Pagination
            currentPage={currentPage}
            totalPages={Math.ceil(totalCount / pageSize)}
            onPageChange={handlePageChange}
          />
        </>
      )}
    </div>
  );
};

export default ModerationUsersList;
