import { useRouter } from "next/router";
import { ssoAsync } from "../features/auth/authSlide";
import { useAppDispatch } from "../hook";

export default function Login() {
  var router = useRouter();
  const dispatch = useAppDispatch();
  return (
    <section>
      <button
        onClick={async () => {
          await dispatch(ssoAsync());
          router.push("/");
        }}
      >
        Login
      </button>
    </section>
  );
}
