"use client";
import { invoke } from "./interop";

export default function Home() {
  const buttonClick = async () => {
    console.log("Button clicked");
    const result = await invoke<string>("GetViews", { Args: "Hello" });

    if (result.type === "ok") {
      console.log("Success:", result.value);
    } else {
      console.error("Error:", result.error);
    }
  };

  const buttonClick2 = async () => {
    console.log("Button clicked 2");
    const result = await invoke<string>("Test", { Args: "Hello" });

    if (result.type === "ok") {
      console.log("Success:", result.value);
    } else {
      console.error("Error:", result.error);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center h-screen">
      <h1 className="text-4xl font-bold">Hello World</h1>
      <button
        className="bg-blue-500 text-white p-2 rounded-md"
        onClick={buttonClick}
      >
        Click me
      </button>
      <button
        className="bg-red-500 text-white p-2 rounded-md"
        onClick={buttonClick2}
      >
        Click me 2
      </button>
    </div>
  );
}
