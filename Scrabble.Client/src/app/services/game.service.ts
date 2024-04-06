import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { Board, LetterChange, Result } from "../models/board";
import { Observable } from "rxjs";
import { SessionService } from "./session.service";

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private hubConnection: HubConnection;

  constructor(private sessionService: SessionService) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7278/game')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public joinGame(sessionKey: string, playerName: string) {
    this.hubConnection.invoke<Result>('JoinGame', sessionKey, playerName)
      .then(result => {
        if (result.isSuccess) {
          console.log('Joined game');
        } else {
          console.error(result.message);
        }
      })
      .catch(err => console.error(err));
  }

  public onLetterChange(): Observable<LetterChange> {
    return new Observable<LetterChange>(observer => {
      this.hubConnection.on('LetterChanged', (letterChange: LetterChange) => {
        observer.next(letterChange);
      });
    });
  }

  public onBoardChange(): Observable<Board> {
    return new Observable<Board>(observer => {
      this.hubConnection.on('BoardChanged', (board: Board) => {
        observer.next(board);
      })
    })
  }

  public onKicked(): Observable<void> {
    return new Observable<void>(observer => {
      this.hubConnection.on('PlayerKicked', () => {
        observer.next();
      })
    })
  }

  public onScoreChange(): Observable<number> {
    return new Observable<number>(observer => {
      this.hubConnection.on('ScoreChanged', (score: number) => {
        observer.next(score);
      })
    })
  }

  public placeLetter(row: number, col: number, value: string | null) {
    this.hubConnection.invoke<Result>('PlaceLetter', this.sessionService.getSessionKey(), row, col, value)
      .then(result => {
        if (result.isSuccess) {
          console.log('Letter placed');
        } else {
          console.error(result.message);
        }
      })
      .catch(err => console.error(err));
  }

  public commitWord(): void {
    let sessionKey = this.sessionService.getSessionKey();
    this.hubConnection.invoke<Result>('CommitWord', sessionKey)
      .then(result => {
        if (result.isSuccess) {
          console.log('Word committed');
        } else {
          console.error(result.message);
        }
      })
      .catch(err => console.error(err));
  }

  public startGame(): void {
    let sessionKey = this.sessionService.getSessionKey();
    this.hubConnection.invoke<Result>('StartGame', sessionKey)
      .then(result => {
        if (result.isSuccess) {
          console.log('Game started');
        } else {
          console.error(result.message);
        }
      })
      .catch(err => console.error(err));
  }
}
