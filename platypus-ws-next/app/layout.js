export const metadata = {
  title: "Realtime WS",
  description: "PlayFab WebSocket endpoint via Next.js Edge Route",
};

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
