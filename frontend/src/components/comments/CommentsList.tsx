
import React, { useState, useEffect } from "react";
import { Comment } from "../../types";
import CommentItem from "./CommentItem";
import { commentsAPI } from "../../services/api";
import { toast } from "sonner";

interface CommentsListProps {
  reviewId: string;
  highlightedCommentId?: string | null;
  onCommentDeleted?: () => void;
  onCommentUpdated?: () => void;
}

interface PaginatedComments {
  items: Comment[];
  totalCount: number;
  page: number;
  pageSize: number;
}

const CommentsList: React.FC<CommentsListProps> = ({ 
  reviewId, 
  highlightedCommentId,
  onCommentDeleted,
  onCommentUpdated
}) => {
  const [comments, setComments] = useState<Comment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const fetchComments = async () => {
    try {
      setLoading(true);
      const response = await commentsAPI.getReviewComments(reviewId);
      
      // Handle the paginated response format
      if (response && response.items && Array.isArray(response.items)) {
        console.log("Comments received successfully:", response);
        setComments(response.items);
      } else {
        console.error("Expected paginated array of comments but received:", response);
        setComments([]);
        setError("Неверный формат данных комментариев");
      }
    } catch (err) {
      console.error("Error fetching comments:", err);
      setComments([]);
      setError("Не удалось загрузить комментарии");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (reviewId) {
      fetchComments();
    } else {
      console.error("No reviewId provided to CommentsList");
      setError("ID обзора отсутствует");
      setLoading(false);
    }
  }, [reviewId]);

  const handleReplyAdded = () => {
    fetchComments();
  };

  const handleCommentDeleted = async (commentId: string) => {
    try {
      await commentsAPI.deleteComment(reviewId, commentId);
      toast.success("Комментарий удален");
      fetchComments();
      if (onCommentDeleted) {
        onCommentDeleted();
      }
    } catch (error) {
      toast.error("Не удалось удалить комментарий");
    }
  };

  const handleCommentUpdated = async (commentId: string, text: string) => {
    try {
      await commentsAPI.updateComment(reviewId, commentId, { text });
      toast.success("Комментарий обновлен");
      fetchComments();
      if (onCommentUpdated) {
        onCommentUpdated();
      }
    } catch (error) {
      toast.error("Не удалось обновить комментарий");
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center py-6">
        <div className="animate-spin h-8 w-8 border-4 border-primary border-t-transparent rounded-full"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-destructive/10 text-destructive p-4 rounded-md">
        {error}
      </div>
    );
  }

  if (!comments || comments.length === 0) {
    return <p className="text-muted-foreground text-center py-4">Нет комментариев</p>;
  }

  return (
    <div className="space-y-4">
      {comments.map((comment) => (
        <CommentItem
          key={comment.id}
          comment={comment}
          reviewId={reviewId}
          isHighlighted={comment.id === highlightedCommentId}
          onReplyAdded={handleReplyAdded}
          onCommentDeleted={handleCommentDeleted}
          onCommentUpdated={handleCommentUpdated}
        />
      ))}
    </div>
  );
};

export default CommentsList;
