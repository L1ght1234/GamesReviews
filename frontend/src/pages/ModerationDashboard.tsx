
import React, { useState } from "react";
import { Link } from "react-router-dom";
import Layout from "../components/layout/Layout";
import { useAuth } from "../context/AuthContext";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { User, UserPlus } from "lucide-react";
import ModerationUsersList from "../components/moderation/ModerationUsersList";
import ModerationReportsList from "../components/moderation/ModerationReportsList";
import CreateModeratorForm from "../components/moderation/CreateModeratorForm";

const ModerationDashboard: React.FC = () => {
  const { isModerator, isAdmin } = useAuth();
  const [activeTab, setActiveTab] = useState("users");

  if (!isModerator) {
    return (
      <Layout>
        <div className="max-w-2xl mx-auto">
          <div className="bg-card border border-border rounded-lg p-6">
            <h1 className="text-xl font-semibold mb-4">Доступ запрещен</h1>
            <p>
              У вас нет прав для доступа к панели модерации.{" "}
              <Link to="/" className="text-primary hover:underline">
                Вернуться на главную
              </Link>
            </p>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-6xl mx-auto">
        <h1 className="text-3xl font-bold mb-6">Панель модерации</h1>

        <Tabs defaultValue={activeTab} onValueChange={setActiveTab}>
          <div className="border-b border-border mb-6">
            <TabsList className="bg-transparent">
              <TabsTrigger 
                value="users" 
                className="data-[state=active]:border-b-2 data-[state=active]:border-primary data-[state=active]:text-primary rounded-none"
              >
                <User size={18} className="mr-2" />
                Пользователи
              </TabsTrigger>
              <TabsTrigger 
                value="reports" 
                className="data-[state=active]:border-b-2 data-[state=active]:border-primary data-[state=active]:text-primary rounded-none"
              >
                <svg 
                  xmlns="http://www.w3.org/2000/svg" 
                  width="18" 
                  height="18" 
                  viewBox="0 0 24 24" 
                  fill="none" 
                  stroke="currentColor" 
                  strokeWidth="2" 
                  strokeLinecap="round" 
                  strokeLinejoin="round" 
                  className="mr-2"
                >
                  <path d="M13.73 21a9.97 9.97 0 0 1-10.68-9.68" />
                  <path d="M21 13.73A10 10 0 0 0 3.35 3.35" />
                  <path d="m10.71 5.04-.28-.65a2 2 0 0 0-3.66 0l-6.04 14.1c-.47 1.1.29 2.36 1.47 2.45l14.58 1a2 2 0 0 0 2.01-2.68l-2.45-5.71" />
                </svg>
                Репорты
              </TabsTrigger>
              {isAdmin && (
                <TabsTrigger 
                  value="addModerator" 
                  className="data-[state=active]:border-b-2 data-[state=active]:border-primary data-[state=active]:text-primary rounded-none"
                >
                  <UserPlus size={18} className="mr-2" />
                  Добавить модератора
                </TabsTrigger>
              )}
            </TabsList>
          </div>

          <TabsContent value="users" className="mt-0">
            <ModerationUsersList />
          </TabsContent>

          <TabsContent value="reports" className="mt-0">
            <ModerationReportsList />
          </TabsContent>

          {isAdmin && (
            <TabsContent value="addModerator" className="mt-0">
              <CreateModeratorForm />
            </TabsContent>
          )}
        </Tabs>
      </div>
    </Layout>
  );
};

export default ModerationDashboard;
