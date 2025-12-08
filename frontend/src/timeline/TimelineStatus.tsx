import "../../sass/timeline.scss";

type TimelineStatusProps = {
  message: string;
  type?: "default" | "error" | "loading";
};

export default function TimelineStatus({
  message,
  type = "default",
}: TimelineStatusProps) {
  return <div className={`timeline-status ${type}`}>{message}</div>;
}
