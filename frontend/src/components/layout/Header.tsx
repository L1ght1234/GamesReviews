
import React, { useState } from "react";
import { Link } from "react-router-dom";
import { Menu, X, User, LogOut, Settings, FileText } from "lucide-react";
import { useAuth } from "../../context/AuthContext";

const Header: React.FC = () => {
  const { isAuthenticated, user, logout, isModerator, isAdmin } = useAuth();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);

  // Обработчик для открытия/закрытия меню пользователя
  const toggleUserMenu = () => {
    setIsUserMenuOpen(!isUserMenuOpen);
  };

  return (
    <header className="bg-card border-b border-border sticky top-0 z-50">
      <div className="container mx-auto px-4 py-4 flex items-center justify-between">
        <Link to="/" className="flex items-center space-x-2">
          <span className="text-xl font-bold text-primary">GameReview</span>
        </Link>

        {/* Desktop Navigation */}
        <nav className="hidden md:flex items-center space-x-6">
          <Link to="/" className="text-foreground hover:text-primary transition-colors">
            Обзоры
          </Link>
          
          {isAuthenticated && (
            <>
              <Link to="/my-reviews" className="text-foreground hover:text-primary transition-colors">
                Мои обзоры
              </Link>
              <Link to="/my-reports" className="text-foreground hover:text-primary transition-colors">
                Мои репорты
              </Link>
            </>
          )}
          
          {isModerator && (
            <Link to="/moderation" className="text-accent hover:text-accent/80 transition-colors font-medium">
              Модерация
            </Link>
          )}
          
          {!isAuthenticated ? (
            <div className="flex space-x-2">
              <Link to="/login" className="btn-secondary">
                Войти
              </Link>
              <Link to="/register" className="btn-primary">
                Регистрация
              </Link>
            </div>
          ) : (
            <div className="relative">
              <button 
                onClick={toggleUserMenu}
                className="flex items-center space-x-2 hover:text-primary transition-colors"
              >
                <span>{user?.userName}</span>
                <User size={20} />
              </button>
              {isUserMenuOpen && (
                <div className="absolute right-0 mt-2 w-48 rounded-md shadow-lg py-1 bg-card border border-border">
                  <Link 
                    to="/profile" 
                    className="block px-4 py-2 text-sm hover:bg-muted transition-colors flex items-center"
                    onClick={() => setIsUserMenuOpen(false)}
                  >
                    <Settings size={16} className="mr-2" />
                    Профиль
                  </Link>
                  <Link 
                    to="/my-reviews" 
                    className="block px-4 py-2 text-sm hover:bg-muted transition-colors flex items-center"
                    onClick={() => setIsUserMenuOpen(false)}
                  >
                    <FileText size={16} className="mr-2" />
                    Мои обзоры
                  </Link>
                  <button 
                    onClick={() => {
                      logout();
                      setIsUserMenuOpen(false);
                    }} 
                    className="block w-full text-left px-4 py-2 text-sm hover:bg-muted transition-colors flex items-center text-destructive"
                  >
                    <LogOut size={16} className="mr-2" />
                    Выйти
                  </button>
                </div>
              )}
            </div>
          )}
        </nav>

        {/* Mobile Menu Button */}
        <button 
          className="md:hidden" 
          onClick={() => setIsMenuOpen(!isMenuOpen)}
        >
          {isMenuOpen ? <X /> : <Menu />}
        </button>
      </div>

      {/* Mobile Menu */}
      {isMenuOpen && (
        <div className="md:hidden bg-card border-t border-border">
          <div className="container mx-auto px-4 py-2 space-y-2">
            <Link 
              to="/" 
              className="block py-2 hover:text-primary"
              onClick={() => setIsMenuOpen(false)}
            >
              Обзоры
            </Link>
            
            {isAuthenticated && (
              <>
                <Link 
                  to="/my-reviews" 
                  className="block py-2 hover:text-primary"
                  onClick={() => setIsMenuOpen(false)}
                >
                  Мои обзоры
                </Link>
                <Link 
                  to="/my-reports" 
                  className="block py-2 hover:text-primary"
                  onClick={() => setIsMenuOpen(false)}
                >
                  Мои репорты
                </Link>
              </>
            )}
            
            {isModerator && (
              <Link 
                to="/moderation" 
                className="block py-2 text-accent font-medium"
                onClick={() => setIsMenuOpen(false)}
              >
                Модерация
              </Link>
            )}
            
            {!isAuthenticated ? (
              <div className="flex flex-col space-y-2 pt-2">
                <Link 
                  to="/login" 
                  className="btn-secondary text-center"
                  onClick={() => setIsMenuOpen(false)}
                >
                  Войти
                </Link>
                <Link 
                  to="/register" 
                  className="btn-primary text-center"
                  onClick={() => setIsMenuOpen(false)}
                >
                  Регистрация
                </Link>
              </div>
            ) : (
              <div className="border-t border-border pt-2 mt-2">
                <div className="py-2 font-medium">{user?.userName}</div>
                <Link 
                  to="/profile" 
                  className="block py-2 hover:text-primary flex items-center"
                  onClick={() => setIsMenuOpen(false)}
                >
                  <Settings size={16} className="mr-2" />
                  Профиль
                </Link>
                <button 
                  onClick={() => {
                    logout();
                    setIsMenuOpen(false);
                  }} 
                  className="w-full text-left py-2 flex items-center text-destructive"
                >
                  <LogOut size={16} className="mr-2" />
                  Выйти
                </button>
              </div>
            )}
          </div>
        </div>
      )}
    </header>
  );
};

export default Header;
