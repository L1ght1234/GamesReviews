
import React from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import Layout from "../components/layout/Layout";
import ReviewForm from "../components/reviews/ReviewForm";
import { reviewsAPI } from "../services/api";
import { CreateReviewRequest } from "../types";
import { useAuth } from "../context/AuthContext";

const CreateReviewPage: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const handleCreateReview = async (data: CreateReviewRequest) => {
    try {
      const response = await reviewsAPI.create(data);
      toast.success("Обзор успешно создан");
      navigate(`/reviews/${response}`);
    } catch (error) {
      toast.error("Не удалось создать обзор");
      throw error;
    }
  };

  if (!isAuthenticated) {
    return (
      <Layout>
        <div className="max-w-2xl mx-auto">
          <div className="bg-card border border-border rounded-lg p-6">
            <h1 className="text-xl font-semibold mb-4">Требуется авторизация</h1>
            <p>Для создания обзора необходимо войти в аккаунт.</p>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-2xl mx-auto">
        <div className="bg-card border border-border rounded-lg shadow-lg p-6">
          <h1 className="text-2xl font-bold mb-6">Создать новый обзор</h1>
          <ReviewForm onSubmit={handleCreateReview} />
        </div>
      </div>
    </Layout>
  );
};

export default CreateReviewPage;
