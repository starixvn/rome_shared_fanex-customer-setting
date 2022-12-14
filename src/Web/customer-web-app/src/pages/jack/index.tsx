import React from "react";
import {NextPage} from "next";
import {AuthGuard} from "../../features/auth/AuthGuard";
import {wrapper} from "../../store";
import {setAccessToken} from "../../features/auth/authSlide";
import {getWeatherForecast} from "../../features/customer/customerApi";

const JackPage: NextPage = ({data}: any) => {

    return (
        <AuthGuard>
            <div>
                <h1>My name is Jack</h1>
            </div>
        </AuthGuard>
    );
}
export const getServerSideProps = wrapper.getServerSideProps(
    (store) => async (context) => {
        var accessToken = context.req.cookies["accessToken"] ?? null;
        store.dispatch(setAccessToken(accessToken));
        if (accessToken) {
            return {
                props: {
                    data: [],
                },
            };
        }

        return {
            props: {
                data: [],
            },
        };
    }
);
export default JackPage;