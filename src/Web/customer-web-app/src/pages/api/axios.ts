import axios from "axios";
import createAuthRefreshInterceptor from "axios-auth-refresh";
import * as cookie from "cookie";
import * as setCookie from "set-cookie-parser";

const AUTH_URL = "http://localhost:5139/api/Auth";
const CUSTOMER_URL = "http://localhost:3001";

const authAxios = axios.create({
  baseURL: AUTH_URL,
});

const customerAxios = axios.create({
  baseURL: CUSTOMER_URL,
  headers: { "Content-Type": "application/json" },
  withCredentials: true,
});

createAuthRefreshInterceptor(customerAxios, (failedRequest) =>
  // 1. First try request fails - refresh the token.
  authAxios.get("/api/refreshToken").then((resp) => {
    // 1a. Clear old helper cookie used in 'authorize.ts' higher order function.
    if (customerAxios.defaults.headers.setCookie) {
      delete customerAxios.defaults.headers.setCookie;
    }
    const { accessToken } = resp.data;
    // 2. Set up new access token
    const bearer = `Bearer ${accessToken}`;
    customerAxios.defaults.headers.Authorization = bearer;

    // 3. Set up new refresh token as cookie
    const responseCookie = setCookie.parse(resp.headers["set-cookie"])[0]; // 3a. We can't just acces it, we need to parse it first.
    customerAxios.defaults.headers.setCookie = resp.headers["set-cookie"]; // 3b. Set helper cookie for 'authorize.ts' Higher order Function.
    customerAxios.defaults.headers.cookie = cookie.serialize(
      responseCookie.name,
      responseCookie.value
    );
    // 4. Set up access token of the failed request.
    failedRequest.response.config.headers.Authorization = bearer;

    // 5. Retry the request with new setup!
    return Promise.resolve();
  })
);

export { authAxios, customerAxios };
