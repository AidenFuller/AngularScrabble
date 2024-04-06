export interface Board {
  size: number;
  cells: Cell[][];
}

export interface Cell {
  value: string | null;
  committedValue: string | null;
  multiplier?: Multiplier;
  row: number;
  col: number;
}

export type Multiplier = 'DL' | 'TL' | 'DW' | 'TW' | null;

export interface Result {
    isSuccess: boolean;
    message: string;
}

export interface TypedResult<T> {
  value?: T;
  isSuccess: boolean;
  message: string;
}

const typedResult: TypedResult<void> = {
  isSuccess: true,
  message: 'Success'
}

export interface LetterChange {
  row: number;
  col: number;
  value: string;
}
