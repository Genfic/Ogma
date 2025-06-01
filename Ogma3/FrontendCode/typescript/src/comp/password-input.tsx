import type { Empty } from "@t/utils";
import { type ComponentType, customElement } from "solid-element";
import { createSignal } from "solid-js";
import { Styled } from "./common/_styled";
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

	const icon = (): string => (showPassword() ? "lucide:eye" : "lucide:eye-closed");

	return (
		<button type="button" class="action-btn show-password" onClick={toggle}>
			<o-icon icon={icon()} />
		</button>
	);
};

customElement("o-password-toggle", {}, Styled(PasswordInput, sharedCss, css));
