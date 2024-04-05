export interface Board {
  size: number;
  cells: Cell[];
}

export interface Cell {
  row: number;
  col: number;
  value?: string;
  multiplier: Multiplier;
}

export type Multiplier = 'None' | 'DoubleLetter' | 'TripleLetter' | 'DoubleWord' | 'TripleWord';

export interface Result {
    isSuccess: boolean;
    message: string;
}

export interface TypedResult<T> {
  result: T;
  isSuccess: boolean;
  message: string;
}

export interface LetterChange {
  row: number;
  col: number;
  value: string | null;
}
