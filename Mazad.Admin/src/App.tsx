import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { LanguageProvider } from "@/contexts/LanguageContext";
import { ThemeProvider } from "@/contexts/ThemeContext";
import Index from "./pages/Index";
import Users from "./pages/Users";
// import Regions from "./pages/Regions";
import Categories from "./pages/Categories";
import NotFound from "./pages/NotFound";
import Subcategories from "./components/Subcategories";
import CategoriesTree from "./components/CategoriesTree";
import DynamicAttributes from "./pages/DynamicAttributes";
import CategoryAttributes from "./components/CategoryAttributes";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <ThemeProvider>
      <TooltipProvider>
        <LanguageProvider>
          <div className="min-h-screen bg-background text-foreground transition-colors">
            <Toaster />
            <Sonner />
            <BrowserRouter>
              <Routes>
                <Route path="/" element={<Index />} />
                <Route path="/users" element={<Users />} />
                {/* <Route path="/regions" element={<Regions />} /> */}
                <Route path="/categories" element={<Categories />} />
                <Route path="/subcategories/:id" element={<Subcategories/>} />
                <Route path="/categories-tree" element={<CategoriesTree />} />
                <Route path="/dynamic-attributes" element={<DynamicAttributes />} />
                <Route path="/attributes/:categoryId" element={<CategoryAttributes />} />

                <Route path="*" element={<NotFound />} />
              </Routes>
            </BrowserRouter>
          </div>
        </LanguageProvider>
      </TooltipProvider>
    </ThemeProvider>
  </QueryClientProvider>
);

export default App;
