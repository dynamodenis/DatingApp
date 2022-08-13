import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'AppFrontend';
  users:any;

  constructor(private http: HttpClient){}
  ngOnInit() {
    this.getUsers();
  }

  getUsers(){
    this.http.get("https://localhost:7095/api/users").subscribe(res => {
      this.users = res;
    }, error  => {
      console.log(error)
    })
  }
}