
import React, { useState } from "react";
import { toast } from "sonner";
import { CreateReviewRequest, UpdateReviewRequest } from "../../types";

interface ReviewFormProps {
  initialData?: {
    gameName: string;
    description: string;
    tags: string[];
  };
  onSubmit: (data: CreateReviewRequest | UpdateReviewRequest) => Promise<void>;
  isEditing?: boolean;
}

const ReviewForm: React.FC<ReviewFormProps> = ({
  initialData = { gameName: "", description: "", tags: [] },
  onSubmit,
  isEditing = false,
}) => {
  const [gameName, setGameName] = useState(initialData.gameName);
  const [description, setDescription] = useState(initialData.description);
  const [tags, setTags] = useState<string[]>(initialData.tags);
  const [tagInput, setTagInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleAddTag = () => {
    const trimmedTag = tagInput.trim();
    if (trimmedTag && !tags.includes(trimmedTag)) {
      setTags([...tags, trimmedTag]);
      setTagInput("");
    } else if (tags.includes(trimmedTag)) {
      toast.error("Этот тег уже добавлен");
    }
  };

  const handleRemoveTag = (tagToRemove: string) => {
    setTags(tags.filter((tag) => tag !== tagToRemove));
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleAddTag();
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!gameName.trim()) {
      toast.error("Укажите название игры");
      return;
    }
    
    if (!description.trim()) {
      toast.error("Добавьте описание обзора");
      return;
    }
    
    setIsLoading(true);
    
    try {
      await onSubmit({
        gameName,
        description,
        tags,
      });
    } catch (error) {
      console.error("Error submitting review:", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div>
        <label htmlFor="gameName" className="block mb-1 text-sm font-medium">
          Название игры
        </label>
        <input
          id="gameName"
          type="text"
          value={gameName}
          onChange={(e) => setGameName(e.target.value)}
          className="w-full bg-muted border border-border rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
          required
        />
      </div>

      <div>
        <label htmlFor="description" className="block mb-1 text-sm font-medium">
          Описание обзора
        </label>
        <textarea
          id="description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="w-full bg-muted border border-border rounded-md px-3 py-2 min-h-[200px] focus:outline-none focus:ring-2 focus:ring-primary"
          required
        />
      </div>

      <div>
        <label htmlFor="tags" className="block mb-1 text-sm font-medium">
          Теги
        </label>
        <div className="flex items-center">
          <input
            id="tags"
            type="text"
            value={tagInput}
            onChange={(e) => setTagInput(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Добавьте тег и нажмите Enter"
            className="flex-1 bg-muted border border-border rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
          />
          <button
            type="button"
            onClick={handleAddTag}
            className="ml-2 btn-secondary"
          >
            Добавить
          </button>
        </div>

        <div className="flex flex-wrap gap-2 mt-3">
          {tags.map((tag, index) => (
            <div
              key={index}
              className="bg-secondary/70 text-foreground px-3 py-1 rounded-full text-sm flex items-center"
            >
              {tag}
              <button
                type="button"
                onClick={() => handleRemoveTag(tag)}
                className="ml-2 text-muted-foreground hover:text-destructive"
              >
                &times;
              </button>
            </div>
          ))}
        </div>
      </div>

      <button 
        type="submit" 
        disabled={isLoading}
        className="btn-primary w-full flex items-center justify-center"
      >
        {isLoading ? (
          <svg 
            className="animate-spin h-5 w-5" 
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
        ) : isEditing ? "Сохранить изменения" : "Создать обзор"}
      </button>
    </form>
  );
};

export default ReviewForm;
