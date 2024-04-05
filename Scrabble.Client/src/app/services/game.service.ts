import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {Board, LetterChange, Result} from "../models/board";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private hubConnection: HubConnection;

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
        .withUrl('https://localhost:7278/game')
        .build();

    this.hubConnection
        .start()
        .then(() => console.log('Connection started'))
        .catch(err => console.log('Error while starting connection: ' + err));
  }

  public joinGame(gameId: string, playerName: string) {
    this.hubConnection.invoke<Result>('JoinGame', gameId, playerName)
        .then(result => {
            if (result.isSuccess) {
                console.log('Joined game');
            }
            else {
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

  public placeLetter(row: number, col: number, value: string) {
    this.hubConnection.invoke<Result>('PlaceLetter', "TEST", row, col, value)
        .then(result => {
            if (result.isSuccess) {
                console.log('Letter placed');
            }
            else {
                console.error(result.message);
            }
        })
        .catch(err => console.error(err));
  }
}
