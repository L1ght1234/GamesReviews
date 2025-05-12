
import React from "react";

const Footer: React.FC = () => {
  return (
    <footer className="bg-card border-t border-border py-6">
      <div className="container mx-auto px-4">
        <div className="flex flex-col md:flex-row justify-between items-center">
          <div className="mb-4 md:mb-0">
            <span className="text-primary font-bold">GameReview</span>
            <p className="text-sm text-muted-foreground mt-1">
              Место для обзоров ваших любимых игр
            </p>
          </div>
          <div className="text-sm text-muted-foreground">
            © {new Date().getFullYear()} GameReview. Все права защищены.
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
