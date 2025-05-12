import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link, useSearchParams } from "react-router-dom";
import { toast } from "sonner";
import Layout from "../components/layout/Layout";
import CommentsList from "../components/comments/CommentsList";
import CommentForm from "../components/comments/CommentForm";
import ReportDialog from "../components/shared/ReportDialog";
import { reviewsAPI, commentsAPI } from "../services/api";
import { Review, ReportedContentType } from "../types";
import { useAuth } from "../context/AuthContext";
import { Flag, Edit, Trash } from "lucide-react";

const ReviewDetailPage: React.FC = () => {
  const { reviewId } = useParams<{ reviewId: string }>();
  const navigate = useNavigate();
  const { isAuthenticated, user, isModerator } = useAuth();
  const [searchParams] = useSearchParams();
  const highlightCommentId = searchParams.get("highlightComment");
  
  const [review, setReview] = useState<Review | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [isReportDialogOpen, setIsReportDialogOpen] = useState(false);
  const [refreshComments, setRefreshComments] = useState(0);

  useEffect(() => {
    const fetchReview = async () => {
      if (!reviewId) {
        console.error("No reviewId provided");
        setError("Не удалось загрузить обзор: ID не предоставлен");
        setLoading(false);
        return;
      }
      
      try {
        setLoading(true);
        console.log(`Загрузка обзора с ID: ${reviewId}`);
        const data = await reviewsAPI.getById(reviewId);
        console.log("Полученные данные обзора:", data);
        
        if (!data) {
          console.error("Review data is empty");
          setError("Обзор не найден");
          setLoading(false);
          return;
        }
        
        setReview(data);
        setError("");
      } catch (err) {
        console.error("Error fetching review:", err);
        setError("Не удалось загрузить обзор");
      } finally {
        setLoading(false);
      }
    };

    fetchReview();
  }, [reviewId]);

  // Вычисляемые свойства для проверки прав
  const isAuthor = isAuthenticated && user?.id === review?.userId;
  const canModify = isAuthor || isModerator;

  const handleAddComment = async (text: string) => {
    if (!reviewId) return;
    
    try {
      await commentsAPI.createComment(reviewId, { text });
      toast.success("Комментарий добавлен");
      setRefreshComments(prev => prev + 1);
    } catch (error) {
      toast.error("Не удалось добавить комментарий");
    }
  };

  const handleDeleteReview = async () => {
    if (!reviewId) return;
    
    if (window.confirm("Вы уверены, что хотите удалить этот обзор?")) {
      try {
        await reviewsAPI.delete(reviewId);
        toast.success("Обзор успешно удален");
        navigate("/");
      } catch (error) {
        toast.error("Не удалось удалить обзор");
      }
    }
  };

  // Effect to scroll to highlighted comment if needed
  useEffect(() => {
    if (highlightCommentId && !loading) {
      // Give time for comments to render
      setTimeout(() => {
        const commentElement = document.getElementById(`comment-${highlightCommentId}`);
        if (commentElement) {
          commentElement.scrollIntoView({ behavior: 'smooth' });
          commentElement.classList.add('bg-primary/10');
          commentElement.classList.add('highlight-pulse');
          
          // Remove highlighting after some time
          setTimeout(() => {
            commentElement.classList.remove('highlight-pulse');
          }, 3000);
        }
      }, 500);
    }
  }, [highlightCommentId, loading, refreshComments]);

  if (loading) {
    return (
      <Layout>
        <div className="flex justify-center py-12">
          <div className="animate-spin h-12 w-12 border-4 border-primary border-t-transparent rounded-full"></div>
        </div>
      </Layout>
    );
  }

  if (error || !review) {
    return (
      <Layout>
        <div className="max-w-4xl mx-auto">
          <div className="bg-destructive/10 text-destructive p-4 rounded-md">
            {error || "Обзор не найден"}
          </div>
          <Link to="/" className="btn-secondary mt-4 inline-block">
            Вернуться на главную
          </Link>
        </div>
      </Layout>
    );
  }

  // Format date nicely
  const formattedDate = new Date(review.createdAt).toLocaleDateString("ru-RU", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });

  return (
    <Layout>
      <div className="max-w-4xl mx-auto">
        <div className="bg-card border border-border rounded-lg shadow-lg p-6 mb-6">
          <div className="flex justify-between items-start">
            <div>
              <h1 className="text-2xl font-bold">{review.gameName}</h1>
              <div className="text-muted-foreground mt-1 mb-3">
                {formattedDate}
              </div>
            </div>
            
            <div className="flex space-x-2">
              {canModify && (
                <>
                  <Link
                    to={`/edit-review/${review.reviewId}`}
                    className="flex items-center space-x-1 btn-secondary"
                  >
                    <Edit size={16} />
                    <span>Изменить</span>
                  </Link>
                  <button
                    onClick={handleDeleteReview}
                    className="flex items-center space-x-1 bg-destructive text-destructive-foreground hover:bg-destructive/90 rounded-md px-4 py-2 font-medium"
                  >
                    <Trash size={16} />
                    <span>Удалить</span>
                  </button>
                </>
              )}
              
              {isAuthenticated && user?.id !== review.userId && (
                <button
                  onClick={() => setIsReportDialogOpen(true)}
                  className="flex items-center space-x-1 text-muted-foreground hover:text-destructive"
                >
                  <Flag size={16} />
                  <span>Пожаловаться</span>
                </button>
              )}
            </div>
          </div>
          
          <div className="my-4 flex flex-wrap gap-2">
            {review.tags && review.tags.map((tag, index) => (
              <span key={index} className="game-tag">
                {tag}
              </span>
            ))}
          </div>
          
          <div className="mt-6 prose prose-invert max-w-none leading-relaxed">
            <p className="whitespace-pre-wrap">{review.description}</p>
          </div>
        </div>
        
        <div className="mb-6">
          <h2 className="text-xl font-semibold mb-4">Комментарии</h2>
          
          {isAuthenticated ? (
            <div className="mb-6">
              <CommentForm
                onSubmit={handleAddComment}
                submitLabel="Добавить комментарий"
              />
            </div>
          ) : (
            <div className="bg-muted p-4 rounded-md mb-6">
              <p className="text-sm text-muted-foreground">
                Чтобы оставлять комментарии, пожалуйста,{" "}
                <Link to="/login" className="text-primary hover:underline">
                  войдите
                </Link>{" "}
                или{" "}
                <Link to="/register" className="text-primary hover:underline">
                  зарегистрируйтесь
                </Link>
                .
              </p>
            </div>
          )}
          
          {reviewId && (
            <CommentsList 
              reviewId={reviewId} 
              key={refreshComments}
              highlightedCommentId={highlightCommentId}
              onCommentDeleted={() => setRefreshComments(prev => prev + 1)}
              onCommentUpdated={() => setRefreshComments(prev => prev + 1)}
            />
          )}
        </div>
        
        {isReportDialogOpen && (
          <ReportDialog
            isOpen={isReportDialogOpen}
            onClose={() => setIsReportDialogOpen(false)}
            contentId={review.reviewId}
            reportedUserId={review.userId}
            contentType={ReportedContentType.Review}
          />
        )}
      </div>
    </Layout>
  );
};

export default ReviewDetailPage;
