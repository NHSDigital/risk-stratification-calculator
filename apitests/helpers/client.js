import dotenv from 'dotenv';
import fetch from 'node-fetch';

export class Client {
  constructor(baseUrl) {
    this.baseUrl = baseUrl;
  }

  async __post(content) {
    const fullUrl = this.getFullUrl(this.baseUrl);

    return fetch(
      fullUrl,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/fhir+json',
          'x-api-key': process.env.CALCENGINE_API_GATEWAY_KEY
        },
        body: JSON.stringify(content)
      }
    );
  }

  async postSuccess(content) {
    const response = await this.__post(content);
    if (response.status !== 200) {
      console.log(await response.json());
    }
    expect(response.status).toBe(200);
    const res = await response.json();
    expect(res.prediction.length).not.toBe(0);
    return res;
  }

  async postError(content) {
    const response = await this.__post(content);
    expect(response.status).toBe(400);
    return response.json();
  }

  async postInternalError(content) {
    const response = await this.__post(content);
    expect(response.status).toBe(500);
    return response.json();
  }

  getFullUrl(url) {
    if (url.startsWith('http://') || url.startsWith('https://')) {
      return url;
    }
    const relative = url.startsWith('/') ? url : `/${url}`;
    const spacer = url.endsWith('/') ? 'qcovid' : '/qcovid';
    return url + spacer + relative;
  }
}

dotenv.config();
const apiEndpoint = process.env.API_ENDPOINT || 'http://localhost:3000/api';
export const qCovidClient = new Client(`${apiEndpoint}/qcovid`);
