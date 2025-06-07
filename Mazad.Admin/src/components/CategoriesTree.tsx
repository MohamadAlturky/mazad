import React, { useState, useEffect } from 'react';
import { ChevronDown, ChevronRight, Edit } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { useLanguage } from '@/contexts/LanguageContext';
import { cn } from '@/lib/utils';
import AdminLayout from './AdminLayout';

interface CategoryItem {
    id: number;
    name: string;
    isActive: boolean;
    children?: CategoryItem[];
}

const CategoriesTree: React.FC = () => {
    const { t, isRTL } = useLanguage();
    const [expandedItems, setExpandedItems] = useState<Set<number>>(new Set());
    const [categories, setCategories] = useState<CategoryItem[]>([]);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await fetch('http://localhost:5032/api/categories', {
                    headers: {
                        'Accept-Language': 'en',
                    },
                });
                const data = await response.json();
                if (data.success) {
                    setCategories(data.data);
                }
            } catch (error) {
                console.error('Error fetching categories:', error);
            }
        };

        fetchCategories();
    }, []);

    const toggleExpanded = (id: number) => {
        const newExpanded = new Set(expandedItems);
        if (newExpanded.has(id)) {
            newExpanded.delete(id);
        } else {
            newExpanded.add(id);
        }
        setExpandedItems(newExpanded);
    };

    const renderCategoryItem = (item: CategoryItem, level: number = 0) => {
        const hasChildren = item.children && item.children.length > 0;
        const isExpanded = expandedItems.has(item.id);
        const paddingValue = level * 24;

        const itemStyle = isRTL
            ? { paddingRight: `${paddingValue + 16}px` }
            : { paddingLeft: `${paddingValue + 16}px` };

        return (
            <div key={item.id}>
                <div
                    className="flex items-center justify-between py-3 px-4 border-b border-purple-100 hover:bg-purple-50 transition-colors"
                    style={itemStyle}
                >
                    <div className="flex items-center gap-3">
                        {hasChildren ? (
                            <button
                                onClick={() => toggleExpanded(item.id)}
                                className="text-purple-600 hover:text-purple-800"
                            >
                                {isExpanded ? (
                                    <ChevronDown className={cn("h-4 w-4", isRTL ? "rotate-180" : "")} />
                                ) : (
                                    <ChevronRight className={cn("h-4 w-4", isRTL ? "rotate-180" : "")} />
                                )}
                            </button>
                        ) : (
                            <div className="w-4" />
                        )}
                        <span className="font-medium text-purple-900">{item.name}</span>
                    </div>

                    <Badge
                        variant={item.isActive === true ? 'default' : 'secondary'}
                        className={item.isActive === true ? 'bg-purple-100 text-purple-800' : 'bg-gray-100 text-gray-800'}
                    >
                        {t(item.isActive === true ? 'active' : 'inactive')}
                    </Badge>
                </div>

                {hasChildren && isExpanded && (
                    <div>
                        {item.children!.map(child => renderCategoryItem(child, level + 1))}
                    </div>
                )}
            </div>
        );
    };

    return (
        <AdminLayout>
            <Card className="border-purple-200">
                <CardHeader>
                    <CardTitle className="text-purple-900"> {t('categories')} </CardTitle>
                </CardHeader>

                <CardContent>
                    <div className="space-y-1">
                        {categories.length === 0
                            ? (
                                <>
                                    <div className="flex flex-col justify-center items-center h-32 text-purple-600">
                                        <svg fill="#7c3aed" version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" width="100px" height="100px" viewBox="0 0 59.227 59.227" xmlSpace="preserve">

                                            <g id="SVGRepo_bgCarrier" stroke-width="0" />

                                            <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round" />

                                            <g id="SVGRepo_iconCarrier"> <g> <g> <path d="M51.586,10.029c-0.333-0.475-0.897-0.689-1.449-0.607c-0.021-0.005-0.042-0.014-0.063-0.017L27.469,6.087 c-0.247-0.037-0.499-0.01-0.734,0.076L8.63,12.799c-0.008,0.003-0.015,0.008-0.023,0.011c-0.019,0.008-0.037,0.02-0.057,0.027 c-0.099,0.044-0.191,0.096-0.276,0.157c-0.026,0.019-0.051,0.038-0.077,0.059c-0.093,0.076-0.178,0.159-0.249,0.254 c-0.004,0.006-0.01,0.009-0.014,0.015L0.289,23.78c-0.293,0.401-0.369,0.923-0.202,1.391c0.167,0.469,0.556,0.823,1.038,0.947 l6.634,1.713v16.401c0,0.659,0.431,1.242,1.062,1.435l24.29,7.422c0.008,0.004,0.017,0.001,0.025,0.005 c0.13,0.036,0.266,0.059,0.402,0.06c0.003,0,0.007,0.002,0.011,0.002l0,0h0.001c0.143,0,0.283-0.026,0.423-0.067 c0.044-0.014,0.085-0.033,0.13-0.052c0.059-0.022,0.117-0.038,0.175-0.068l17.43-9.673c0.477-0.265,0.772-0.767,0.772-1.312 V25.586l5.896-2.83c0.397-0.19,0.69-0.547,0.802-0.973c0.111-0.427,0.03-0.88-0.223-1.241L51.586,10.029z M27.41,9.111 l17.644,2.59L33.35,17.143l-18.534-3.415L27.41,9.111z M9.801,15.854l21.237,3.914l-6.242,9.364l-20.78-5.365L9.801,15.854z M10.759,43.122V28.605l14.318,3.697c0.125,0.031,0.25,0.048,0.375,0.048c0.493,0,0.965-0.244,1.248-0.668l5.349-8.023v25.968 L10.759,43.122z M49.479,41.1l-14.431,8.007V25.414l2.635,5.599c0.171,0.361,0.479,0.641,0.854,0.773 c0.163,0.06,0.333,0.087,0.502,0.087c0.223,0,0.444-0.05,0.649-0.146l9.789-4.698L49.479,41.1L49.479,41.1z M39.755,28.368 l-4.207-8.938L49.85,12.78l5.634,8.037L39.755,28.368z" /> </g> </g> </g>

                                        </svg>
                                        {t('noDataAvailable')}
                                    </div>
                                </>
                            ) : (
                                categories.map(item => renderCategoryItem(item))
                            )}
                    </div>
                </CardContent>
            </Card>
        </AdminLayout>
    );
};

export default CategoriesTree;