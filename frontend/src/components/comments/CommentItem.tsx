
import React, { useState, useEffect } from "react";
import { Comment } from "../../types";
import { useAuth } from "../../context/AuthContext";
import { commentsAPI } from "../../services/api";
import { toast } from "sonner";
import CommentForm from "./CommentForm";
import { ReportedContentType } from "../../types";
import ReportDialog from "../shared/ReportDialog";
import { Edit, Trash, Reply, Flag } from "lucide-react";

interface CommentItemProps {
  comment: Comment;
  reviewId: string;
  isHighlighted?: boolean;
  onReplyAdded: () => void;
  onCommentDeleted: (commentId: string) => void;
  onCommentUpdated: (commentId: string, text: string) => void;
  isReply?: boolean;
}

const CommentItem: React.FC<CommentItemProps> = ({
  comment,
  reviewId,
  isHighlighted = false,
  onReplyAdded,
  onCommentDeleted,
  onCommentUpdated,
  isReply = false,
}) => {
  const { user, isAuthenticated, isModerator } = useAuth();
  const [showReplyForm, setShowReplyForm] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [replies, setReplies] = useState<Comment[]>([]);
  const [showReplies, setShowReplies] = useState(false);
  const [loadingReplies, setLoadingReplies] = useState(false);
  const [showReportDialog, setShowReportDialog] = useState(false);
  const [hasReplies, setHasReplies] = useState(false);

  const isAuthor = user?.id === comment.userId;
  const canModify = isAuthor || isModerator;

  const formattedDate = new Date(comment.createdAt).toLocaleDateString("ru-RU", {
    day: "numeric",
    month: "long",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });

  // Check if comment has replies (only needed for root comments)
  useEffect(() => {
    if (isReply) return;
    
    const checkReplies = async () => {
      try {
        const response = await commentsAPI.getReplies(reviewId, comment.id);
        if (response && response.items && Array.isArray(response.items)) {
          setHasReplies(response.items.length > 0);
        }
      } catch (error) {
        console.error("Error checking for replies:", error);
      }
    };
    
    checkReplies();
  }, [reviewId, comment.id, isReply]);

  // Auto-expand replies if this comment is the highlighted one
  useEffect(() => {
    if (isHighlighted && !isReply && !showReplies && hasReplies) {
      loadReplies();
    }
  }, [isHighlighted, isReply, showReplies, hasReplies]);

  const handleReplySubmit = async (text: string) => {
    if (!text.trim()) return;

    try {
      await commentsAPI.createReply(reviewId, comment.id, { text });
      setShowReplyForm(false);
      toast.success("Ответ добавлен");
      onReplyAdded();
      
      if (!isReply) {
        setHasReplies(true);
        if (showReplies) {
          loadReplies();
        }
      }
    } catch (error) {
      toast.error("Не удалось добавить ответ");
    }
  };

  const handleUpdateSubmit = async (text: string) => {
    if (!text.trim()) return;

    try {
      await commentsAPI.updateComment(reviewId, comment.id, { text });
      setIsEditing(false);
      onCommentUpdated(comment.id, text);
    } catch (error) {
      toast.error("Не удалось обновить комментарий");
    }
  };

  const loadReplies = async () => {
    if (isReply) return;

    try {
      setLoadingReplies(true);
      const response = await commentsAPI.getReplies(reviewId, comment.id);
      
      if (response && response.items && Array.isArray(response.items)) {
        setReplies(response.items);
        setHasReplies(response.items.length > 0);
      } else {
        console.error("Expected paginated array of replies but received:", response);
        setReplies([]);
      }
      
      setShowReplies(true);
    } catch (error) {
      toast.error("Не удалось загрузить ответы");
      setReplies([]);
    } finally {
      setLoadingReplies(false);
    }
  };

  const handleToggleReplies = () => {
    if (showReplies) {
      setShowReplies(false);
    } else {
      loadReplies();
    }
  };

  // Add highlight-pulse animation CSS class
  const highlightClass = isHighlighted
    ? "bg-primary/10 highlight-pulse"
    : "";

  return (
    <div 
      id={`comment-${comment.id}`}
      className={`bg-card border border-border rounded-lg p-4 ${isReply ? "ml-8" : ""} ${highlightClass}`}
    >
      <div className="flex justify-between">
        <div className="font-medium">{comment.userName}</div>
        <div className="text-xs text-muted-foreground">{formattedDate}</div>
      </div>

      {isEditing ? (
        <CommentForm
          initialText={comment.text}
          onSubmit={handleUpdateSubmit}
          onCancel={() => setIsEditing(false)}
          submitLabel="Сохранить"
        />
      ) : (
        <p className="mt-2 text-sm">{comment.text}</p>
      )}

      <div className="mt-3 flex flex-wrap items-center gap-4 text-xs">
        {isAuthenticated && !isEditing && (
          <button
            onClick={() => setShowReplyForm(!showReplyForm)}
            className="flex items-center text-muted-foreground hover:text-primary"
          >
            <Reply size={14} className="mr-1" /> Ответить
          </button>
        )}

        {canModify && !isEditing && (
          <>
            <button
              onClick={() => setIsEditing(true)}
              className="flex items-center text-muted-foreground hover:text-accent"
            >
              <Edit size={14} className="mr-1" /> Изменить
            </button>
            <button
              onClick={() => onCommentDeleted(comment.id)}
              className="flex items-center text-muted-foreground hover:text-destructive"
            >
              <Trash size={14} className="mr-1" /> Удалить
            </button>
          </>
        )}

        {isAuthenticated && user?.id !== comment.userId && !isEditing && (
          <button
            onClick={() => setShowReportDialog(true)}
            className="flex items-center text-muted-foreground hover:text-destructive"
          >
            <Flag size={14} className="mr-1" /> Пожаловаться
          </button>
        )}

        {!isReply && hasReplies && (
          <button
            onClick={handleToggleReplies}
            className="ml-auto text-muted-foreground hover:text-primary"
          >
            {showReplies ? "Скрыть ответы" : "Показать ответы"}
          </button>
        )}
      </div>

      {showReplyForm && (
        <div className="mt-4">
          <CommentForm
            onSubmit={handleReplySubmit}
            onCancel={() => setShowReplyForm(false)}
            submitLabel="Ответить"
            placeholder="Напишите ваш ответ..."
          />
        </div>
      )}

      {loadingReplies && (
        <div className="flex justify-center py-2 mt-3">
          <div className="animate-spin h-4 w-4 border-2 border-primary border-t-transparent rounded-full"></div>
        </div>
      )}

      {showReplies && replies.length > 0 && (
        <div className="mt-4 space-y-3">
          {replies.map((reply) => (
            <CommentItem
              key={reply.id}
              comment={reply}
              reviewId={reviewId}
              isHighlighted={reply.id === highlightedCommentId}
              onReplyAdded={onReplyAdded}
              onCommentDeleted={onCommentDeleted}
              onCommentUpdated={onCommentUpdated}
              isReply={true}
            />
          ))}
        </div>
      )}

      {showReportDialog && (
        <ReportDialog
          isOpen={showReportDialog}
          onClose={() => setShowReportDialog(false)}
          contentId={comment.id}
          reportedUserId={comment.userId}
          contentType={ReportedContentType.Comment}
        />
      )}
    </div>
  );
};

export default CommentItem;
