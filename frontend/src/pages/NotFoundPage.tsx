
import React from "react";
import { Link } from "react-router-dom";
import Layout from "../components/layout/Layout";

const NotFoundPage: React.FC = () => {
  return (
    <Layout>
      <div className="flex flex-col items-center justify-center py-12">
        <h1 className="text-6xl font-bold text-primary mb-4">404</h1>
        <p className="text-xl mb-8">Страница не найдена</p>
        <Link to="/" className="btn-primary">
          На главную
        </Link>
      </div>
    </Layout>
  );
};

export default NotFoundPage;
