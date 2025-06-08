export interface Category extends BaseTable {
  name: string;
  isActive: boolean;
}
export interface BaseTable {
  id: number;
  isActive: boolean;
}

export interface DynamicAttribute extends BaseTable {
  name: string;
  type: string;
}

export interface Region extends BaseTable {
  name: string;
  status: 'active' | 'inactive';
  createdAt: string;
}

export interface RegionFormData {
  name: string;
  isActive: boolean;
}

