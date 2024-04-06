import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {BoardComponent} from "./components/board/board.component";
import {CookieService} from "ngx-cookie-service";
import {GameService} from "./services/game.service";
import { MenuComponent } from "./components/menu/menu.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, BoardComponent, MenuComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'AngularPlayground';
}
