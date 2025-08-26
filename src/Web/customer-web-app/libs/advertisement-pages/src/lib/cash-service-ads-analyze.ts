import axios from "axios";

export function AnalyzeCashServiceAds(props: {
    name: string;
    step: number | undefined;
    act: string;
}) {
    const { name, step, act } = props;
    const callAPi = axios.get(`/main/Dashboard/AnalyzeCashServiceAds?Name=${name}&Step=${step}&act=${act}`);
    return callAPi;
}