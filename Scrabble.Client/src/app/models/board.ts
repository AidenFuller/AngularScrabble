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
