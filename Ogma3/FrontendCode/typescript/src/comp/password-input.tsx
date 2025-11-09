import { component } from "@h/web-components";
import type { Empty } from "@t/utils";
import type { ComponentType } from "solid-element";
import { createSignal } from "solid-js";
import { LucideEye } from "../icons/LucideEye";
import { LucideEyeClosed } from "../icons/LucideEyeClosed";
import css from "./password-input.css";
import sharedCss from "./shared.css";

const PasswordInput: ComponentType<Empty> = (_, { element }) => {
	if (!(element instanceof Element)) throw Error("Not an element?");
	const input = element.previousElementSibling as HTMLInputElement;

	const [showPassword, setShowPassword] = createSignal(false);

	const toggle = () => {
		setShowPassword(!showPassword());
		input.type = showPassword() ? "text" : "password";
	};

	return (
		<button type="button" class="action-btn show-password" onClick={toggle}>
			{showPassword() ? <LucideEye /> : <LucideEyeClosed />}
		</button>
	);
};

component("o-password-toggle", {}, PasswordInput, [sharedCss, css]);
