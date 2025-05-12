
import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { Report, ReportStatus, ReportedContentType } from "../../types";
import { reportsAPI } from "../../services/api";

interface ModerationReportItemProps {
  report: Report;
  onStatusUpdated: () => void;
  getStatusName: (status: ReportStatus) => string;
}

const ModerationReportItem: React.FC<ModerationReportItemProps> = ({
  report,
  onStatusUpdated,
  getStatusName,
}) => {
  const [isUpdating, setIsUpdating] = useState(false);
  const [isNavigating, setIsNavigating] = useState(false);
  const navigate = useNavigate();

  const formattedDate = new Date(report.createdAt).toLocaleDateString("ru-RU", {
    day: "numeric",
    month: "long",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });

  const handleStatusChange = async (newStatus: ReportStatus) => {
    if (report.status === newStatus) return;
    
    setIsUpdating(true);
    try {
      await reportsAPI.updateStatus(report.id, { status: newStatus });
      toast.success("Статус жалобы обновлен");
      onStatusUpdated();
    } catch (error) {
      toast.error("Не удалось обновить статус жалобы");
    } finally {
      setIsUpdating(false);
    }
  };

  const getStatusBadgeClass = (status: ReportStatus) => {
    switch (status) {
      case ReportStatus.InProgress:
        return "bg-primary/20 text-primary";
      case ReportStatus.Resolved:
        return "bg-green-500/20 text-green-500";
      case ReportStatus.Dismissed:
        return "bg-orange-500/20 text-orange-500";
      default:
        return "bg-muted text-muted-foreground";
    }
  };

  const navigateToContent = async () => {
    try {
      setIsNavigating(true);
      
      // Fetch the content using the new endpoint
      const content = await reportsAPI.getReportedContent(report.contentType, report.contentId);
      
      console.log("Retrieved content:", content);
      
      // For review content
      if (report.contentType === ReportedContentType.Review) {
        navigate(`/reviews/${content.reviewId || report.contentId}`);
      } 
      // For comment content
      else if (report.contentType === ReportedContentType.Comment) {
        // With the updated CommentResponse DTO, we now have reviewId directly
        if (content.reviewId) {
          // Navigate to review page with a query param to highlight the comment
          navigate(`/reviews/${content.reviewId}?highlightComment=${report.contentId}`);
        } else {
          toast.error("Не удалось определить обзор для этого комментария");
        }
      }
    } catch (error) {
      console.error("Error navigating to content:", error);
      toast.error("Не удалось перейти к контенту");
    } finally {
      setIsNavigating(false);
    }
  };

  return (
    <div className="bg-card border border-border rounded-lg p-4">
      <div className="flex flex-wrap justify-between items-start gap-4 mb-3">
        <div>
          <span className={`px-2 py-1 rounded text-xs ${getStatusBadgeClass(report.status)}`}>
            {getStatusName(report.status)}
          </span>
          <span className="ml-2 text-xs text-muted-foreground">{formattedDate}</span>
        </div>
        <div className="flex items-center space-x-2">
          <span className="text-xs text-muted-foreground mr-2">Изменить статус:</span>
          <button
            onClick={() => handleStatusChange(ReportStatus.InProgress)}
            disabled={isUpdating || report.status === ReportStatus.InProgress}
            className={`px-2 py-1 text-xs rounded ${
              report.status === ReportStatus.InProgress
                ? "bg-primary/20 text-primary"
                : "bg-muted hover:bg-primary/20 hover:text-primary"
            }`}
          >
            В рассмотрении
          </button>
          <button
            onClick={() => handleStatusChange(ReportStatus.Resolved)}
            disabled={isUpdating || report.status === ReportStatus.Resolved}
            className={`px-2 py-1 text-xs rounded ${
              report.status === ReportStatus.Resolved
                ? "bg-green-500/20 text-green-500"
                : "bg-muted hover:bg-green-500/20 hover:text-green-500"
            }`}
          >
            Решено
          </button>
          <button
            onClick={() => handleStatusChange(ReportStatus.Dismissed)}
            disabled={isUpdating || report.status === ReportStatus.Dismissed}
            className={`px-2 py-1 text-xs rounded ${
              report.status === ReportStatus.Dismissed
                ? "bg-orange-500/20 text-orange-500"
                : "bg-muted hover:bg-orange-500/20 hover:text-orange-500"
            }`}
          >
            Отклонено
          </button>
        </div>
      </div>

      <div className="mb-3">
        <h3 className="font-medium">Причина:</h3>
        <p className="text-sm">{report.reason}</p>
      </div>

      {report.description && (
        <div className="mb-3">
          <h3 className="font-medium">Описание:</h3>
          <p className="text-sm whitespace-pre-wrap">{report.description}</p>
        </div>
      )}

      <div className="mb-3">
        <h3 className="font-medium">Тип контента:</h3>
        <p className="text-sm">
          {report.contentType === ReportedContentType.Review ? "Обзор" : "Комментарий"}
        </p>
      </div>

      <div className="mt-4 pt-3 border-t border-border">
        <button
          onClick={navigateToContent}
          disabled={isNavigating}
          className="w-full bg-primary/10 hover:bg-primary/20 text-primary font-medium py-2 rounded-md transition-colors disabled:opacity-50"
        >
          {isNavigating ? (
            <div className="flex justify-center items-center">
              <div className="animate-spin h-4 w-4 border-2 border-primary border-t-transparent rounded-full mr-2"></div>
              Загрузка...
            </div>
          ) : (
            "Перейти к контенту"
          )}
        </button>
      </div>
    </div>
  );
};

export default ModerationReportItem;
