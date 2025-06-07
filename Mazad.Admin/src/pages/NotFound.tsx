import { useLocation } from "react-router-dom";
import { useEffect } from "react";

const NotFound = () => {
  const location = useLocation();

  useEffect(() => {
    console.error(
      "404 Error: User attempted to access non-existent route:",
      location.pathname
    );
  }, [location.pathname]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100">
      <div className="text-center px-4 py-8 max-w-2xl mx-auto">
        <div className="animate-float mb-8">
          <svg
            className="w-64 h-64 mx-auto"
            viewBox="0 0 400 300"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M200 50C122.453 50 60 112.453 60 190C60 267.547 122.453 330 200 330C277.547 330 340 267.547 340 190C340 112.453 277.547 50 200 50ZM200 290C144.771 290 100 245.229 100 190C100 134.771 144.771 90 200 90C255.229 90 300 134.771 300 190C300 245.229 255.229 290 200 290Z"
              fill="#E5E7EB"
            />
            <path
              d="M200 130C177.909 130 160 147.909 160 170C160 192.091 177.909 210 200 210C222.091 210 240 192.091 240 170C240 147.909 222.091 130 200 130Z"
              fill="#9CA3AF"
            />
            <path
              d="M140 170C140 147.909 157.909 130 180 130C202.091 130 220 147.909 220 170C220 192.091 202.091 210 180 210C157.909 210 140 192.091 140 170Z"
              fill="#6B7280"
            />
            <path
              d="M180 170C180 158.954 188.954 150 200 150C211.046 150 220 158.954 220 170C220 181.046 211.046 190 200 190C188.954 190 180 181.046 180 170Z"
              fill="#4B5563"
            />
            <path
              d="M200 250C188.954 250 180 241.046 180 230H220C220 241.046 211.046 250 200 250Z"
              fill="#9CA3AF"
            />
          </svg>
        </div>
        <h1 className="text-6xl font-bold mb-4 text-gray-800 animate-fade-in">
          404
        </h1>
        <p className="text-2xl text-gray-600 mb-8 animate-slide-up">
          Oops! The page you're looking for doesn't exist
        </p>
        <a
          href="/"
          className="inline-block px-8 py-3 bg-blue-600 text-white rounded-lg 
                     hover:bg-blue-700 transform hover:scale-105 transition-all 
                     duration-300 ease-in-out shadow-lg hover:shadow-xl"
        >
          Return to Home
        </a>
      </div>

      <style>
        {`
          @keyframes float {
            0% {
              transform: translateY(0px);
            }
            50% {
              transform: translateY(-20px);
            }
            100% {
              transform: translateY(0px);
            }
          }

          @keyframes fadeIn {
            from {
              opacity: 0;
            }
            to {
              opacity: 1;
            }
          }

          @keyframes slideUp {
            from {
              transform: translateY(20px);
              opacity: 0;
            }
            to {
              transform: translateY(0);
              opacity: 1;
            }
          }

          .animate-float {
            animation: float 6s ease-in-out infinite;
          }

          .animate-fade-in {
            animation: fadeIn 1s ease-out;
          }

          .animate-slide-up {
            animation: slideUp 1s ease-out;
          }
        `}
      </style>
    </div>
  );
};

export default NotFound;