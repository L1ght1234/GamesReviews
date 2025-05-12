
import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { toast } from "sonner";
import Layout from "../components/layout/Layout";
import ReviewForm from "../components/reviews/ReviewForm";
import { reviewsAPI } from "../services/api";
import { UpdateReviewRequest } from "../types";
import { useAuth } from "../context/AuthContext";

const EditReviewPage: React.FC = () => {
  const { reviewId } = useParams<{ reviewId: string }>();
  const navigate = useNavigate();
  const { user, isAuthenticated, isModerator } = useAuth();
  
  const [initialData, setInitialData] = useState<{
    gameName: string;
    description: string;
    tags: string[];
  } | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchReview = async () => {
      if (!reviewId) return;
      
      try {
        setLoading(true);
        const data = await reviewsAPI.getById(reviewId);
        
        // Check if user is authorized to edit
        if (!isModerator && data.userId !== user?.id) {
          setError("У вас нет прав для редактирования этого обзора");
          return;
        }
        
        setInitialData({
          gameName: data.gameName,
          description: data.description,
          tags: data.tags,
        });
      } catch (err) {
        console.error("Error fetching review:", err);
        setError("Не удалось загрузить обзор");
      } finally {
        setLoading(false);
      }
    };

    if (isAuthenticated) {
      fetchReview();
    }
  }, [reviewId, user?.id, isModerator, isAuthenticated]);

  const handleUpdateReview = async (data: UpdateReviewRequest) => {
    if (!reviewId) return;
    
    try {
      await reviewsAPI.update(reviewId, data);
      toast.success("Обзор успешно обновлен");
      navigate(`/reviews/${reviewId}`);
    } catch (error) {
      toast.error("Не удалось обновить обзор");
      throw error;
    }
  };

  if (!isAuthenticated) {
    return (
      <Layout>
        <div className="max-w-2xl mx-auto">
          <div className="bg-card border border-border rounded-lg p-6">
            <h1 className="text-xl font-semibold mb-4">Требуется авторизация</h1>
            <p>Для редактирования обзора необходимо войти в аккаунт.</p>
          </div>
        </div>
      </Layout>
    );
  }

  if (loading) {
    return (
      <Layout>
        <div className="flex justify-center py-12">
          <div className="animate-spin h-12 w-12 border-4 border-primary border-t-transparent rounded-full"></div>
        </div>
      </Layout>
    );
  }

  if (error || !initialData) {
    return (
      <Layout>
        <div className="max-w-2xl mx-auto">
          <div className="bg-destructive/10 text-destructive p-4 rounded-md">
            {error || "Обзор не найден"}
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-2xl mx-auto">
        <div className="bg-card border border-border rounded-lg shadow-lg p-6">
          <h1 className="text-2xl font-bold mb-6">Редактировать обзор</h1>
          <ReviewForm 
            initialData={initialData} 
            onSubmit={handleUpdateReview} 
            isEditing={true}
          />
        </div>
      </div>
    </Layout>
  );
};

export default EditReviewPage;
