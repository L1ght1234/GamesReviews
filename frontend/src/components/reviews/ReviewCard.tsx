
import React from "react";
import { Link } from "react-router-dom";
import { Review } from "../../types";

interface ReviewCardProps {
  review: Review;
}

const ReviewCard: React.FC<ReviewCardProps> = ({ review }) => {
  // Format date nicely
  const formattedDate = new Date(review.createdAt).toLocaleDateString("ru-RU", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });

  return (
    <Link 
      to={`/reviews/${review.reviewId}`}
      className="block bg-card border border-border hover:border-primary transition-colors rounded-lg overflow-hidden"
    >
      <div className="p-5">
        <h3 className="text-xl font-semibold mb-2 text-foreground line-clamp-2">
          {review.gameName}
        </h3>
        
        <div className="text-muted-foreground text-sm mb-3">
          {formattedDate}
        </div>
        
        <p className="text-muted-foreground mb-4 line-clamp-3">
          {review.description}
        </p>
        
        <div className="flex flex-wrap gap-2">
          {review.tags.map((tag, index) => (
            <span key={index} className="game-tag">
              {tag}
            </span>
          ))}
        </div>
      </div>
    </Link>
  );
};

export default ReviewCard;
