
import React, { useState } from "react";
import { toast } from "sonner";
import { ReportedContentType, CreateReportRequest } from "../../types";
import { reportsAPI } from "../../services/api";

interface ReportDialogProps {
  isOpen: boolean;
  onClose: () => void;
  contentId: string;
  reportedUserId: string;
  contentType: ReportedContentType;
}

const ReportDialog: React.FC<ReportDialogProps> = ({
  isOpen,
  onClose,
  contentId,
  reportedUserId,
  contentType,
}) => {
  const [reason, setReason] = useState("");
  const [description, setDescription] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!reason.trim()) {
      toast.error("Укажите причину жалобы");
      return;
    }
    
    setIsLoading(true);
    
    const reportData: CreateReportRequest = {
      reportedUserId,
      contentId,
      contentType,
      reason,
      description,
    };
    
    try {
      await reportsAPI.create(reportData);
      toast.success("Жалоба отправлена");
      onClose();
    } catch (error) {
      console.error("Error creating report:", error);
      toast.error("Не удалось отправить жалобу");
    } finally {
      setIsLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 bg-black/50 flex items-center justify-center p-4">
      <div className="bg-card border border-border rounded-lg shadow-lg max-w-md w-full">
        <div className="p-6">
          <h2 className="text-xl font-semibold mb-4">Отправить жалобу</h2>
          
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label htmlFor="reason" className="block mb-1 text-sm font-medium">
                Причина жалобы*
              </label>
              <input
                id="reason"
                type="text"
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                required
                className="w-full bg-muted border border-border rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            
            <div>
              <label htmlFor="description" className="block mb-1 text-sm font-medium">
                Дополнительное описание (необязательно)
              </label>
              <textarea
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                className="w-full bg-muted border border-border rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary min-h-[100px]"
              />
            </div>
            
            <div className="flex justify-end space-x-2 pt-2">
              <button
                type="button"
                onClick={onClose}
                className="btn-secondary"
                disabled={isLoading}
              >
                Отмена
              </button>
              <button
                type="submit"
                className="btn-destructive bg-destructive text-destructive-foreground hover:bg-destructive/90 px-4 py-2 rounded-md font-medium"
                disabled={isLoading}
              >
                {isLoading ? (
                  <div className="flex items-center">
                    <svg 
                      className="animate-spin h-4 w-4 mr-2" 
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
                    Отправить
                  </div>
                ) : "Отправить"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ReportDialog;
