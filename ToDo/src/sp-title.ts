import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-fetch-client';

@autoinject
export class Users {
  public webTitle: string;

  constructor(private http: HttpClient) {
    http.configure(config => {
      config
        .useStandardConfiguration()
        .withBaseUrl('https://localhost:44336/');
    });
  }

  public activate() {
    return this.http.fetch('api/sp/1')
      .then(response => response.json())
      .then(webTitle => this.webTitle = webTitle);
  }

  public addAurelia()
  {
    var myHeaders = new Headers();

    var myInit = { method: 'post',
               headers:  {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  },
               mode: 'same-origin',
               cache: 'no-cache',
              body: JSON.stringify({
    value: 'myvalue'
  })};

    this.http.fetch('api/sp',myInit)
      // .then(response => response.json())
      // .then(webTitle => this.webTitle = webTitle);
  }

  public removeAurelia()
  {
    

    var myInit = { method: 'delete',
               headers:  {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  },
               mode: 'same-origin',
               cache: 'no-cache',
              body: JSON.stringify({
    id: 1
  })};

  this.http.fetch('api/sp/1',myInit);
      // .then(response => response.json())
      // .then(webTitle => this.webTitle = webTitle);
  }
}
