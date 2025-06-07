export interface Category extends BaseTable {
  name: string;
  isActive: boolean;
}
export interface BaseTable {
  id: number;
  isActive: boolean;
}

